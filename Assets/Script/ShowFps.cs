using UnityEngine;
using System.Collections;

public class ShowFps : MonoBehaviour 
{
	public float updateRate = 4.0f; // 4 updates per sec.
    
	int frameCount = 0;
    float fps = 0;
	float nextUpdate = 0;
    
	GUIStyle guiStyle1 = new GUIStyle();
	GUIContent fpsContent = new GUIContent();
	
	void Start()
	{
		guiStyle1.normal.textColor = Color.white;
		guiStyle1.fontSize = 18;
	}
	
    void Update()
    {
	    frameCount++;
		
	    if (Time.time > nextUpdate)
	    {
		    nextUpdate = Time.time + 1.0f/updateRate;
		    fps = frameCount * updateRate;
		    frameCount = 0;
	    }
    }

#if DEBUG_GUI
	void OnGUI()
	{
		fpsContent.text = "FPS: " + (int)fps;		
		GUI.Label(new Rect((Screen.width/2)-50,Screen.height-30,100,50), fpsContent, guiStyle1);
	}
#endif
}
