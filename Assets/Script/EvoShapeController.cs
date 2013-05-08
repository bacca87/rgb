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
	public InputMode inputMode = InputMode.SWIPE;
	
	private GameManager manager;
		
	// Use this for initialization
	void Start() 
	{
		manager = (GameManager)FindObjectOfType(typeof(GameManager));
	}
		
	// Update is called once per frame
	void FixedUpdate()
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
				//if there was a touch and the first touch's phase represents a finger moving on the screen, manage that movement.
				if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) 
				{
				//apply rotation given by x swipe
					if(Input.GetTouch(0).position.y < 240)
						transform.Rotate(Vector3.forward * Input.GetTouch(0).deltaPosition.x * Time.deltaTime * speed);
					else transform.Rotate(-Vector3.forward * Input.GetTouch(0).deltaPosition.x * Time.deltaTime * speed);
				// apply rotation given by y swipe
					if(Input.GetTouch(0).position.x > 400)
						transform.Rotate(Vector3.forward * Input.GetTouch(0).deltaPosition.y * Time.deltaTime * speed);
					else transform.Rotate(-Vector3.forward * Input.GetTouch(0).deltaPosition.y * Time.deltaTime * speed);
				// TO DO: adjust values basing on screen width, height and on the position of the object on the screen
				// OR: rotate making the object always look to the point pressed by the user
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
