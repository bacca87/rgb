using UnityEngine;
using System.Collections;

public class EvoShape : MonoBehaviour 
{
	public AudioClip good;
	public AudioClip wrong;
	public GameObject explosionPrefab;
	
	private GameManager manager;
	private int esLayer = 0;
		
	void Start ()
	{
		manager = (GameManager)FindObjectOfType(typeof(GameManager));
		esLayer = LayerMask.NameToLayer("EnergyShape");
	}
		
	void OnCollisionEnter(Collision collision)
	{
		//se non è un es oppure è in atto l'animazione dell'evoluzione ignoro le collisioni
		if(collision.gameObject.layer != esLayer || manager.isEvolving)
		{
			Destroy(collision.gameObject);
			return;
		}
		
		bool ok = false;
		ContactPoint point = collision.contacts[0];
		EnergyShape es = collision.gameObject.GetComponent<EnergyShape>();
	
		switch(point.thisCollider.name)
		{
		case "Red":
			if(es.color == CustomColor.Red)
				ok = true;
			break;
			
		case "Green":
			if(es.color == CustomColor.Green)
				ok = true;
			break;
			
		case "Blue":
			if(es.color == CustomColor.Blue)
				ok = true;
			break;
			
		case "Yellow":
			if(es.color == CustomColor.Yellow)
				ok = true;
			break;
		
		case "Orange":
			if(es.color == CustomColor.Orange)
				ok = true;
			break;
			
		case "Cyan":
			if(es.color == CustomColor.Cyan)
				ok = true;
			break;
		
		case "Pink":
			if(es.color == CustomColor.Pink)
				ok = true;
			break;
		
		case "Violet":
			if(es.color == CustomColor.Violet)
				ok = true;
			break;
		}
		
		if(ok)
		{
			manager.increase(es);
			manager.totalESCatched++;
			audio.PlayOneShot(good);
			
			StartCoroutine(esExplosionEffect(es.color, point, collision));
		}
		else
		{
			manager.damage(es);
			manager.totalESMissed++;
			audio.PlayOneShot(wrong);			
			
			#if !UNITY_STANDALONE
			Handheld.Vibrate();
			#endif
			
			if(!animation.isPlaying) animation.Play("Vibrate");
		}
		
		ObjectPool.Instance.PoolObject(collision.gameObject);
	}
	
	IEnumerator esExplosionEffect(Color color, ContactPoint point, Collision collision)
	{
		//esplosione es			
		GameObject explosion = ObjectPool.Instance.GetObjectForType("ESExplosionEffect", true);
		explosion.transform.position = point.point;
		explosion.transform.rotation = Quaternion.FromToRotation(Vector3.forward, collision.collider.transform.position.normalized);
		explosion.particleSystem.startColor = color;
		
		yield return new WaitForSeconds(explosion.particleSystem.duration);
		
		ObjectPool.Instance.PoolObject(explosion);
	}
}
