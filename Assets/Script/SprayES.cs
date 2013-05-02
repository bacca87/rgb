using UnityEngine;
using System.Collections;

public class SprayES : MonoBehaviour 
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
	
	public float speed = 3;
	public float maxDistance = 20;
	public float destroyTime = 5;
	public int totES = 5;
	
	private int sprayCount = 0;
	private RGBSocial social;
	
	// Use this for initialization
	void Start () 
	{
		social = (RGBSocial)FindObjectOfType(typeof(RGBSocial));
	}
	
	// Update is called once per frame
	void Update ()
	{
		#if UNITY_ANDROID
		sprayES();
		#endif
	}
	
	void sprayES()
	{
		for(int touch = 0; touch < Input.touchCount; touch++)
		{
			if (Input.GetTouch(touch).phase == TouchPhase.Began) 
			{
		        Vector2 touchPosition = Input.GetTouch(touch).position;
				Vector3 point = Camera.main.ScreenToWorldPoint(touchPosition);
				Color color = getRandomColor();
				
				for(int i = 0; i < 360; i += 360/totES)
				{
					EnergyShape es = createES(point);
					
					es.destination = getPosition(point,i,maxDistance);
					es.color = color;
					es.timeToCenter = speed;
				}
				
				social.checkSprayCount(++sprayCount);
		    }
		}
	}
	
	EnergyShape createES(Vector2 position)
	{
		GameObject bulletObj = Instantiate(getRandomESPrefab(), position, Quaternion.identity) as GameObject;
		bulletObj.transform.parent = transform.parent;
		Destroy(bulletObj, destroyTime);
		return bulletObj.GetComponent<EnergyShape>();
	}
	
	GameObject getRandomESPrefab()
	{
		switch(Random.Range(3,9))
		{
		case 3:
			return energyShapePrefabs.triangle;
		case 4:
			return energyShapePrefabs.square;
		case 5:
			return energyShapePrefabs.pentagon;
		case 6:
			return energyShapePrefabs.hexagon;
		case 7:
			return energyShapePrefabs.heptagon;			
		case 8:
			return energyShapePrefabs.octagon;
		default:
			return energyShapePrefabs.triangle;
		}		
	}
	
	Vector2 getPosition(Vector3 startPoint, float angle, float radius)
	{
		return new Vector2(startPoint.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad), startPoint.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad));
	}
	
	Color getRandomColor()
	{	
		return CustomColor.List[Random.Range(0, CustomColor.List.Length)];
	}
}
