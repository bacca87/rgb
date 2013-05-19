using UnityEngine;
using System.Collections;

public enum Shape
{
	Triangle = 3,
	Square = 4,
	Pentagon = 5,
	Hexagon = 6,
	Heptagon = 7,
	Octagon = 8
}

public class GameManager : MonoBehaviour
{
	[System.Serializable]
	public class EvolvingRequirements
	{
		public int square = 100;
		public int pentagon = 100;
		public int hexagon = 100;
		public int heptagon = 100;
		public int octagon = 100;
	}
	public EvolvingRequirements evolvingRequirements = new EvolvingRequirements();
	
	[System.Serializable]
	public class Combo
	{
		public int x2 = 20;
		public int x3 = 30;
		public int x4 = 40;
		public int x5 = 50;
	}
	public Combo combo = new Combo();
	
	[System.Serializable]
	public class Explosion
	{
		public float radius = 11;
		public float power = 200;
		public float esDestroyTime = 3;
		public Vector2 position = Vector2.zero;
	}
	public Explosion explosion = new Explosion();
	
	public int scorePoints { get; internal set; }
	public int evolutionPoints { get; internal set; }
	public int comboPoints { get; internal set; }
	public int comboMultiplier { get; internal set; }
	public int evolutionRate { get { return (evolutionPoints*100)/getEvolvingRequirements(currentShape); } }
	public bool endGame { get; internal set; }
	public bool showScore { get; set; }
	public bool updateHUD { get; set; }
	public bool disableInputs { get; internal set; }
	public bool isEvolving { get; internal set; }
	
	public int maxHealthPoints = 100;
	public bool gamePaused = false;
	public bool godMode = false;
	
	public Shape currentShape {get; internal set;}
	public Shape firstShape = Shape.Triangle;
	public Shape lastShape = Shape.Octagon;
			
	private GameObject currentEvoShape;
	private ScreenOrientation lastOrientation;
	private RGBSocial social;
	
	private float m_healthPoints;
	public float healthPoints 
	{ 
		get
		{
			return m_healthPoints;
		}
		
		internal set
		{
			// considero lo stato 0.5 dello sliceamount come il valore massimo per i danni massimi, 
			// se viene raggiunto il restante 0.5 lo dissolve l'animazione della morte
			m_healthPoints = value;
			if(currentEvoShape == null) return;
			float sliceAmount = (((maxHealthPoints-value)*100f/maxHealthPoints)*0.5f)/100f;
			currentEvoShape.renderer.material.SetFloat("_SliceAmount", sliceAmount);
		}
	}
	
	// Common stats 
	public int totalESSpawned { get; set; }
	public int totalESCatched { get; set; }
	public int totalESMissed { get; set; }
	public int totalCombo { get; set; }
	public int totalEvolutions { get; set; }
	
	// Time stats
	public float totalGameTime { get; set; }
	
	// Use this for initialization
	void Start ()
	{
		scorePoints = 0;
		comboPoints = 0;
		healthPoints = maxHealthPoints;
		comboMultiplier = 1;
		evolutionPoints = 0;
		endGame = false;
		showScore = false;
		updateHUD = true;
		disableInputs = false;
		isEvolving = false;
		gamePaused = false;
		lastOrientation = Screen.orientation;
		social = (RGBSocial)FindObjectOfType(typeof(RGBSocial));
		
		currentShape = firstShape;
		currentEvoShape = ObjectPool.Instance.GetObjectForType("Evo3", true);		
	}
	
	// Update is called once per frame
	void Update ()
	{
		checkLoss();
		checkEvolution();
		
		if (!gamePaused && !endGame)
		{
			totalGameTime += Time.deltaTime;
			social.checkSessionTime(totalGameTime);
			social.checkTotalPlayTime(Time.deltaTime);
		}
		
		if(lastOrientation != Screen.orientation)
		{
			lastOrientation = Screen.orientation;
			social.checkRotation();
		}
	}
	
	void Pause ()
	{
		gamePaused = true;
		disableInputs = true;
	}
	
	void Resume()
	{
		gamePaused = false;
		disableInputs = false;
	}
	
	public void increase(EnergyShape es)
	{
		if(endGame) return;
		
		// COMBO POINTS
		comboPoints++;

		if(comboPoints == combo.x2)
		{
			comboMultiplier = 2;
			totalCombo++;
		}
		if(comboPoints == combo.x3)
		{
			comboMultiplier = 3;
		}
		if(comboPoints == combo.x4)
		{
			comboMultiplier = 4;
		}
		if(comboPoints >= combo.x5)
		{
			comboMultiplier = 5;
		}
				
		// EVOLUTION POINTS
		evolutionPoints++;
		
		if(evolutionPoints > getEvolvingRequirements(lastShape))
			evolutionPoints = getEvolvingRequirements(lastShape);
		
		// SCORE POINTS
		scorePoints += (int)currentShape * (int)es.moveSpeed * comboMultiplier;
		
		social.checkCombo(comboMultiplier);
		social.checkCatchedES();
		
		updateHUD = true;
	}
	
	public void damage(EnergyShape es)
	{
		stopCombo();
		
		if(godMode)
			return;
		
		healthPoints--;
	}
	
	void stopCombo()
	{
		comboPoints = 0;
		comboMultiplier = 1;
	}
			
	void checkLoss()
	{
		if(!endGame && !godMode && healthPoints < 1)
		{
			endGame = disableInputs = true;
			
			// TODO: blocca lo spawner (come quando si mette in pausa)
			
			social.submitScore(scorePoints);
			social.checkZeroPointDeath(evolutionPoints);
			social.checkCompletedGames();
			social.save();
			
			StartCoroutine(endGameEffect());
		}
	}
	
	void checkEvolution()
	{
		if(currentShape == lastShape) return;
				
		if(evolutionPoints >= getEvolvingRequirements(currentShape))
		{
			// EVOLUZIONE
			evolutionPoints = 0;
			healthPoints = maxHealthPoints;
			totalEvolutions++;
			
			currentShape += 1;
			
			social.checkEvolution(currentShape, totalESMissed > 0);
						
			StartCoroutine(evolutionEffect(currentEvoShape));
		}
	}
	
	IEnumerator evolutionEffect(GameObject evoshape)
	{
		isEvolving = true;
		
		if(evoshape != null)
		{
			evoshape.animation.Play("FastRotate");			
			yield return new WaitForSeconds(1.5f);
			evoshape.animation.Stop("FastRotate");
			
			StartCoroutine(explosionEffect());
			ObjectPool.Instance.PoolObject(currentEvoShape); //elimino vecchia evo
			currentEvoShape = ObjectPool.Instance.GetObjectForType(getEvoShapePrefab(currentShape), true);//setto nuova evo
		}
		
		isEvolving = false;
	}
	
	IEnumerator explosionEffect()
	{
		//effetto esplosione che spazza via tutte le es
		Collider[] colliders = Physics.OverlapSphere(explosion.position, explosion.radius);
		for(int i = 0; i < colliders.Length; i++)
		{
			Collider hit = colliders[i];
			
			if(!hit) continue;
			
			EnergyShape es = hit.GetComponent<EnergyShape>();
			
			if(!es) continue;
				
			es.stop = true;
			hit.rigidbody.AddExplosionForce(explosion.power, explosion.position, explosion.radius);
		}
		
		GameObject explosionObj = ObjectPool.Instance.GetObjectForType("EvoExplosionEffect", true);
		explosionObj.particleSystem.startColor = CustomColor.List[(int)currentShape-1];
		
		yield return new WaitForSeconds(explosionObj.particleSystem.duration);
		
		for(int i = 0; i < colliders.Length; i++)
		{
			Collider hit = colliders[i];
			
			if(!hit) continue;
			
			EnergyShape es = hit.GetComponent<EnergyShape>();
			
			if(!es) continue;
			
			ObjectPool.Instance.PoolObject(hit.gameObject);
		}
		
		ObjectPool.Instance.PoolObject(explosionObj);
	}
	
	IEnumerator endGameEffect()
	{
		currentEvoShape.animation.Play("EvoShapeDeath");
		
		#if !UNITY_STANDALONE
		Handheld.Vibrate();
		#endif
				
		yield return new WaitForSeconds(currentEvoShape.animation.GetClip("EvoShapeDeath").length);
		
		Destroy(currentEvoShape);
		
		showScore = true;
	}
	
	string getEvoShapePrefab(Shape shape)
	{
		switch(shape)
		{
		case Shape.Square:
			return "Evo4";
		case Shape.Pentagon:
			return "Evo5";			
		case Shape.Hexagon:
			return "Evo6";
		case Shape.Heptagon:
			return "Evo7";
		case Shape.Octagon:
			return "Evo8";
		default:
			return null;
		}
	}
	
	int getEvolvingRequirements(Shape shape)
	{
		switch(shape)
		{
		case Shape.Triangle:
			return evolvingRequirements.square;
		case Shape.Square:
			return evolvingRequirements.pentagon;
		case Shape.Pentagon:
			return evolvingRequirements.hexagon;			
		case Shape.Hexagon:
			return evolvingRequirements.heptagon;			
		case Shape.Heptagon:
			return evolvingRequirements.octagon;
		case Shape.Octagon:
			return evolvingRequirements.octagon;
		default:
			return 0;
		}
	}
}
