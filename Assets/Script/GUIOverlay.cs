using UnityEngine;
using System.Collections;

public class GUIOverlay : MonoBehaviour
{
	GameManager manager;
	
	GUIStyle guiStyle1 = new GUIStyle();
	GUIContent score = new GUIContent();
	GUIContent evolution = new GUIContent();
	GUIContent combo = new GUIContent();
	GUIContent time = new GUIContent();
	GUIContent healt = new GUIContent();
	
	// Use this for initialization
	void Start () 
	{
		manager = (GameManager)FindObjectOfType(typeof(GameManager));
		
		guiStyle1.normal.textColor = Color.white;
		guiStyle1.fontSize = 18;		
	}
	
	// Update is called once per frame
	void Update()
	{
	}

#if DEBUG_GUI
	void OnGUI()
	{
		time.text = "Time: " + (int)manager.totalGameTime + "s";
		score.text = "Score: " + manager.scorePoints;
		combo.text = "Combo: " + manager.comboMultiplier + "x";
		evolution.text = "Evolution: " + manager.evolutionRate + "%";
		healt.text = "Health: " + manager.healthPoints;
		
		GUI.Label(new Rect(600,40,100,50), time, guiStyle1);
		GUI.Label(new Rect(600,60,100,50), score, guiStyle1);
		GUI.Label(new Rect(600,80,100,50), combo, guiStyle1);
		GUI.Label(new Rect(600,100,100,50), evolution, guiStyle1);	
		GUI.Label(new Rect(600,120,100,50), healt, guiStyle1);
	}
#endif
}
