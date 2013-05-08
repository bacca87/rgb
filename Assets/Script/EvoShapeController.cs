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
	private Vector3 previousTouchPosition = Vector3.zero;
	
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
			transform.Rotate(0,0,Input.GetAxisRaw("Vertical") * speed * Time.deltaTime);		
#elif UNITY_ANDROID
			switch(inputMode)
			{
			case InputMode.SWIPE:
			{
				if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) 
				{
		            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
					float deltaMove = Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y)? touchDeltaPosition.x : touchDeltaPosition.y;				
		            transform.Rotate(0, 0, -deltaMove); //da decidere se limitare la velocità Mathf.Clamp(-deltaMove * speed, -speed, speed)
		        }
			}
			break;
				
			case InputMode.ROTATION:
			{
				if (Input.GetTouch(0).phase == TouchPhase.Began) 
				{
					previousTouchPosition = camera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f));
			    }
				
				if (Input.GetTouch(0).phase == TouchPhase.Moved) 
				{
					Vector3 touchPosition = camera.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f));;
					Vector2 diff = touchPosition - previousTouchPosition;
					float angle = Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg;
					transform.RotateAround(transform.position, Vector3.up, angle);
					previousTouchPosition = touchPosition;
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
