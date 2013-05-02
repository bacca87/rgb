using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Menu2DManager : MonoBehaviour 
{	
	public Menu2DPage activePage;
	
	// Check a touch over any collider
	void touchCheck() 
	{
		if (Input.touches.Length !=0) 
		{
			foreach (Touch touch in Input.touches) 
			{
				// Create a raycast to check the button selected and notify it
				Ray ray = Camera.main.ScreenPointToRay(touch.position);
				RaycastHit hit;
				Physics.Raycast (ray, out hit);
				
				if (touch.phase == TouchPhase.Began) 
				{
					hit.transform.SendMessage("onSelected");
	        	}
				else if (touch.phase == TouchPhase.Moved)
				{
					// If the indicator of a slider was touched, notify the movement (or simply move it here).
					if(hit.transform.GetComponent<Menu2DSliderIndicator>() != null)
						hit.transform.SendMessage("onMoved", Input.GetTouch(0).deltaPosition);
				}
				else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Ended) 
				{
					// do nothing
				}
	    	}
		}
	}
		
	// Method to change page
	public IEnumerator changePage(Menu2DPage page) 
	{
		// Play PageOut animation on the active page
		activePage.animation.Play("PageOut");
		
		// Wait for the animation to finish (optional)
		yield return new WaitForSeconds(activePage.animation.GetClip("PageOut").length);
		
		// Hide the old page (TO DO)
		
		// Set the new page position to correctly roll in (TO DO)
		
		// Load the new page
		page.animation.Play("PageIn");
		
		// Set the new active page
		activePage = page;
	}
	
	// Method to change scene
	public IEnumerator loadScene(string sceneName) 
	{
		// Play FadeOut animation of the active page if there is one?
		activePage.animation.Play("FadeOut");
		
		// Hide the old page (TO DO)
		
		// Wait for the animation to finish
		yield return new WaitForSeconds(activePage.animation.GetClip("FadeOut").length);
		
		// Change the scene
		Application.LoadLevel(sceneName);
	}
	
	public void quitApplication() 
	{
		Application.Quit();
	}
	
	// Use this for initialization
	void Start () 
	{
		// Load the active page with the proper animation
		activePage.transform.animation.Play("FadeIn");
	}
	
	// Update is called once per frame
	void Update () 
	{
		touchCheck();	
	}
}
