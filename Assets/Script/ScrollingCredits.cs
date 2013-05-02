using UnityEngine;
using System.Collections;

public class ScrollingCredits : MonoBehaviour 
{
	public float speed = 10;	
	public float yStart = -100;
	public float yMax = 100;
	
	delegate void ScrollingEffect();
	
	ScrollingEffect scrolling;
	
	// Use this for initialization
	void Start () 
	{
		transform.position = new Vector3(0,yStart,0);
		scrolling = autoScrolling;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.touchCount > 0)
		{
			switch(Input.GetTouch(0).phase)
			{
			case TouchPhase.Began:
				scrolling = manualScrolling;
				break;
			case TouchPhase.Ended:
				scrolling = autoScrolling;
				break;
			default:
				break;
			}
		}
		
		scrolling();
	}
	
	void autoScrolling()
	{
		if(transform.position.y <= yMax && transform.position.y >= yStart)
			transform.Translate(0, speed * Time.deltaTime, 0);
		else
			transform.position = new Vector3(0,yStart,0);
	}
	
	void manualScrolling()
	{
		if (Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			transform.Translate(0, Input.GetTouch(0).deltaPosition.y * Time.deltaTime, 0);
		}
	}
}
