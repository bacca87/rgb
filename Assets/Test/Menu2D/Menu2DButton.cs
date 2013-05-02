using UnityEngine;
using System.Collections;

public enum ButtonFunction 
{
	LoadScene,
	ChangePage,
	QuitApplication
}

[System.Serializable]
public class Menu2DButton : MonoBehaviour 
{	
	public ButtonFunction buttonFunction;
	public string sceneName;
	public Menu2DPage nextpage;
	
	// Method executed when the button is selected.
	public IEnumerator onSelected() 
	{	
		// Play the touch animation (default animation)
		if (!transform.parent.animation.isPlaying) 
		{
			animation.Play();
		}
		
//		// Wait for the animation to finish
		yield return new WaitForSeconds(animation.clip.length);
		
		// Execute the button actions
		switch(buttonFunction)
		{
		case ButtonFunction.LoadScene:
			// notify the parent objects that a change of scene has been called
			SendMessageUpwards("loadScene", sceneName);
			break;
			
		case ButtonFunction.ChangePage:
			// notify the parent objects that a change of menu page has been called
			SendMessageUpwards("changePage", nextpage);
			break;
		
		case ButtonFunction.QuitApplication:
			SendMessageUpwards("quitApplication", nextpage);			
			break;
		}
	}
}
