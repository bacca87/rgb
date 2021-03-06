using UnityEngine;
using System.Collections;

public class EvoShapeController : MonoBehaviour 
{	
	[AddComponentMenu("Character/MoveCharacter")]
	//[RequireComponent("")]
	
	public enum InputMode
	{	
		SWIPE = 0,
		ROTATION = 1
	}
	
	public float speed = 1;
	public bool isActive = true;
	public InputMode inputMode = InputMode.ROTATION;
	
	private Vector3 touchBeganRotation;
   	private Vector2 rotationAxis;
   	private float touchBeganAngle;
	
	private GameManager manager;
		
	// Use this for initialization
	void Start() 
	{
		manager = (GameManager)FindObjectOfType(typeof(GameManager));
	}
		
	// Update is called once per frame
	void Update() 
	{
		if(!manager.disableInputs)
		{
#if UNITY_STANDALONE
			if (Input.GetMouseButtonDown(0)) 
			{
				touchBeganRotation = -transform.eulerAngles;
   				rotationAxis = Camera.main.WorldToScreenPoint(transform.position);
   				Vector2 position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - rotationAxis;
   				touchBeganAngle = Mathf.Atan2(position.y, position.x);
			}	
			else if (Input.GetMouseButton(0)) 
			{
				Vector2 position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - rotationAxis;
    			float angle = Mathf.Atan2(position.y, position.x);
   				Vector3 rotation = touchBeganRotation;
    			rotation.z = -(rotation.z - (angle - touchBeganAngle) * Mathf.Rad2Deg);
   				transform.eulerAngles = rotation;
		    }
			
#elif UNITY_ANDROID
			switch(inputMode)
			{
			case InputMode.SWIPE:
			{
				if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) 
				{
		            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
					float deltaMove = Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y)? touchDeltaPosition.x : touchDeltaPosition.y;				
		            transform.Rotate(0, 0, -deltaMove); 
		        }
			}
			break;
				
			case InputMode.ROTATION:
			{
				if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
				{
					touchBeganRotation = -transform.eulerAngles;
   					rotationAxis = Camera.main.WorldToScreenPoint(transform.position);
   					Vector2 position = Input.GetTouch(0).position - rotationAxis;
   					touchBeganAngle = Mathf.Atan2(position.y, position.x);
			    }
				
				else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) 
				{
					Vector2 position = Input.GetTouch(0).position - rotationAxis;
    				float angle = Mathf.Atan2(position.y, position.x);
   					Vector3 rotation = touchBeganRotation;
    				rotation.z = -(rotation.z - (angle - touchBeganAngle) * Mathf.Rad2Deg);
   					transform.eulerAngles = rotation;
		        }
			}
			break;
				
			default:
				break;
			}
#endif
		}
	}
}
