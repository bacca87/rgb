using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Positioning
{
	Standard,
	Parallel,
	Perpendicular
}

public class Spawner : MonoBehaviour 
{
	[System.Serializable]
	public class EnergyShapePrefabs
	{
		public GameObject triangle;
		public GameObject square;
		public GameObject pentagon;
		public GameObject hexagon;
		public GameObject heptagon;
		public GameObject octagon;
	}
	public EnergyShapePrefabs energyShapePrefabs = new EnergyShapePrefabs();
	
	[System.Serializable]
	public class Rates
	{
		[System.Serializable]
		public class PositioningRate
		{
			public float standard = 50;
			public float parallel = 30;
			public float perpendicular = 20;
		}
		
		[System.Serializable]
		public class MovementRate
		{
			public float normal = 50;
			public float spyral = 20;
			public float sineWave = 30;
		}
		
		[System.Serializable]
		public class SpeedRate
		{
			public float normal = 80;
			public float fast = 20;
		}
		
		[System.Serializable]
		public class SpecialRate
		{
			public float burst = 10;
		}
		
		public PositioningRate positioning = new PositioningRate();
		public MovementRate movement = new MovementRate();
		public SpeedRate speed = new SpeedRate();
		public SpecialRate special = new SpecialRate();
	}
	public Rates rates = new Rates();
	
	[System.Serializable]
	public class TimeToCenter
	{
		public float normal = 10;
		public float fast = 3;
	}
	public TimeToCenter timeToCenter = new TimeToCenter();
	
	public int minBurst = 3;
	public int maxBurst = 10;
	public float burstOffset = 0.5f;
	
	public AnimationCurve spawnOffset = new AnimationCurve(new Keyframe(0, 5), new Keyframe(600, 1));
	public float timeStep = 0.5f;
	public int bufferSize = 5;
	public float radius = 10;
	public Vector2 center = Vector2.zero;
	
	private GameManager manager;
	private float lastTime;
		
	struct ESInfo
	{
		public Positioning positioning;
		public MoveType movetype;
		public float timeToCenter;
		public int count;
		public float spawnTime;
		public int verse;
	}
	
	private List<ESInfo> spawnBuffer = new List<ESInfo>();
	private float timeSequence = 0;
	private float timeSinceGameStart = 0;
	
	// Use this for initialization
	void Start ()
	{
		manager = (GameManager)FindObjectOfType(typeof(GameManager));
		lastTime = Time.time;
		
		//inizializzo il buffer di spawn
		checkBuffer(true);
		checkSpawn(); //controllo per spawnare la prima ES
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!manager.gamePaused)
		{
			timeSinceGameStart += Time.deltaTime;
			
			if(Time.time - lastTime >= timeStep)
			{
				lastTime = Time.time;			
				checkSpawn();
			}
		}
	}
	
	void checkSpawn()
	{
		for(int i = 0; i < spawnBuffer.Count; i++)
		{
			if(timeSinceGameStart >= spawnBuffer[i].spawnTime)
			{
				spawn(spawnBuffer[i]);
				spawnBuffer.RemoveAt(i);
			}
		}
		
		checkBuffer();
	}
	
	void spawn(ESInfo info)
	{
		EnergyShape[] esList = null; 
		
		//posizionamento
		switch(info.positioning)
		{
		case Positioning.Standard:
			esList = standard(info.count);
			break;
		case Positioning.Parallel:
			esList = parallel(info.count);
			break;
		case Positioning.Perpendicular:
			esList = perpendicular(info.count);
			break;
		}
		
		//comportamento
		for(int i = 0; i < esList.Length; i++)
		{
			EnergyShape es = esList[i];
			es.moveType = info.movetype;
			es.timeToCenter = info.timeToCenter;
			es.isFast = info.timeToCenter == timeToCenter.fast ? true : false;
			es.spiralAngularSpeed = es.spiralAngularSpeed * info.verse; // verse è 1 o -1 quindi determina il verso di rotazione
		}
	}
	
	// il parametro init serve per capire se è il primo checkbuffer che inizializza la lista per inizializzare il 
	// timeSequence in base alla prima ES
	void checkBuffer(bool init = false)
	{
		for(int i = spawnBuffer.Count; i < bufferSize; i++)
		{
			ESInfo info = new ESInfo();
			info.positioning = getRandomPositioning();
			info.movetype = getRandomMoveType();
			info.timeToCenter = getRandomTimeToCenter();
			info.count = getRandomBurst();
			info.verse = getRandomVerse();
			
			if(init)
			{
				init = false; // da fare solo per la prima ES
				timeSequence = info.timeToCenter;
			}
			
			//setto quando dovrà spawnare
			info.spawnTime = timeSequence - info.timeToCenter;
			//calcolo l'offset e lo aggiungo alla sequenza temporale per il calcolo del momento di spawn della prossima es
			timeSequence += spawnOffset.Evaluate(timeSinceGameStart) + (info.count - 1) * burstOffset;
			
			spawnBuffer.Add(info);
		}
	}
	
	Positioning getRandomPositioning()
	{
		float random = Random.Range(0.0f, 100.0f);
		
		if(random <= rates.positioning.standard)
			return Positioning.Standard;
		else if( random <= rates.positioning.standard + rates.positioning.parallel)
			return Positioning.Parallel;
		else
			return Positioning.Perpendicular;
	}
	
	MoveType getRandomMoveType()
	{
		float random = Random.Range(0.0f, 100.0f);
		
		if(random <= rates.movement.normal)
			return MoveType.Normal;
		else if( random <= rates.movement.normal + rates.movement.sineWave)
			return MoveType.SineWawe;
		else
			return MoveType.Spyral;
	}
	
	float getRandomTimeToCenter()
	{
		float random = Random.Range(0.0f, 100.0f);
		
		if(random <= rates.speed.normal)
			return timeToCenter.normal;
		else 
			return timeToCenter.fast;
	}
	
	int getRandomBurst()
	{
		float random = Random.Range(0.0f, 100.0f);
		
		if(random <= rates.special.burst)
			return Random.Range(minBurst, maxBurst);
		else 
			return 1;
	}
	
	int getRandomVerse()
	{
		float random = Random.Range(0.0f, 100.0f);
		
		if(random <= 50.0f)
			return 1;
		else 
			return -1;
	}
	
	EnergyShape createES(Vector2 position)
	{
		GameObject bulletObj = Instantiate(getEnergyShapePrefab(manager.currentShape), position, Quaternion.identity) as GameObject;
				
		if(manager.endGame) 
			Destroy(bulletObj, timeToCenter.normal * 2); //alla fine del gioco autodistruggo le es dopo 20 secondi per non sovraccaricare la memoria
		else
			manager.totalESSpawned++; // for every ES created, not just every spawn procedure executed (can spawn multiple ES at a time)
		
		return bulletObj.GetComponent<EnergyShape>();
	}
	
	float getRandomAngle()
	{
		return Random.Range(0, 360);
	}
	
	Vector2 getPosition(float angle = -1, float customRadius = -1)
	{
		angle = angle != -1 ? angle : getRandomAngle();
		customRadius = customRadius != -1 ? customRadius : radius;
		
		return new Vector2(center.x + customRadius * Mathf.Cos(angle * Mathf.Deg2Rad), center.y + customRadius * Mathf.Sin(angle * Mathf.Deg2Rad));
	}
	
	Color getRandomColor()
	{	
		return CustomColor.List[Random.Range(0, (int)manager.currentShape)];
	}
	
	GameObject getEnergyShapePrefab(Shape shape)
	{
		switch(shape)
		{
		case Shape.Triangle:
			return energyShapePrefabs.triangle;
		case Shape.Square:
			return energyShapePrefabs.square;
		case Shape.Pentagon:
			return energyShapePrefabs.pentagon;
		case Shape.Hexagon:
			return energyShapePrefabs.hexagon;
		case Shape.Heptagon:
			return energyShapePrefabs.heptagon;			
		case Shape.Octagon:
			return energyShapePrefabs.octagon;
		default:
			return null;
		}
	}
	
	EnergyShape[] standard(int burst = 1)
	{		
		EnergyShape[] list = new EnergyShape[burst];
		float angle = getRandomAngle();
		Color color = getRandomColor();
		
		for(int i = 0; i < burst; i++)
		{		
			EnergyShape es = createES(getPosition(angle));
			es.color = color;
			es.sleepTime = i * burstOffset;
			list[i] = es;
		}
		
		return list;
	}
		
	EnergyShape[] parallel(int burst = 1)
	{
		EnergyShape[] list = new EnergyShape[2*burst];
		
		float angle1 = getRandomAngle();
		float angle2 = angle1 + 3f;		
		Color color = getRandomColor();
		Vector2 pos1 = getPosition(angle1);
		Vector2 pos2 = getPosition(angle2);		
		Vector2 offset = pos1 - getPosition((angle1+angle2)/2);
		Vector2 offset2 = pos2 - getPosition((angle1+angle2)/2);
		
		for(int i = 0, j = 0; i < burst; i++, j+=2)
		{	
			EnergyShape es = createES(pos1);
			es.color = color;			
			es.destination = offset;
			es.sleepTime = i * burstOffset;
			
			EnergyShape es2 = createES(pos2);
			es2.color = color;		
			es2.destination = offset2;
			es2.sleepTime = i * burstOffset;
			
			list[j] = es;
			list[j+1] = es2;
		}
		
		return list;
	}
	
	EnergyShape[] perpendicular(int burst = 1)
	{
		EnergyShape[] list = new EnergyShape[(int)manager.currentShape * burst];
		float start = getRandomAngle();
		float offset = 360 / (int)manager.currentShape;
		
		for(int i = 0; i < burst; i++)
		{
			for(int j = 0; j < (int)manager.currentShape; j++)
			{
				float angle = start - offset * j;
				if(angle < 0) angle += 360f;			
				
				EnergyShape es = createES(getPosition(angle));
				es.color = CustomColor.List[j];
				es.sleepTime = i * burstOffset;
				
				list[i*(int)manager.currentShape+j] = es;
			}
		}
		
		return list;
	}
}
