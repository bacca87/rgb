using UnityEngine;
using System.Collections;

public class GUIOverlay : MonoBehaviour
{
	private GameManager manager;
	
	// Use this for initialization
	void Start () 
	{
		manager = (GameManager)FindObjectOfType(typeof(GameManager));
	}
	
	// Update is called once per frame
	void Update()
	{
	}
	
	void OnGUI()
	{
		GUIStyle style1 = new GUIStyle();
		GUIStyle style2 = new GUIStyle();
		GUIContent score = new GUIContent();
		GUIContent evolution = new GUIContent();
		GUIContent combo = new GUIContent();
		GUIContent time = new GUIContent();
		GUIContent healt = new GUIContent();
		
		style1.normal.textColor = Color.white;
		style1.fontSize = 18;
		style2.normal.textColor = Color.black;
		style2.fontSize = 18;
		
		time.text = "Time: " + (int)manager.totalGameTime + "s";
		score.text = "Score: " + manager.scorePoints;
		combo.text = "Combo: " + manager.comboMultiplier + "x";
		evolution.text = "Evolution: " + manager.evolutionRate + "%";
		healt.text = "Health: " + manager.healthPoints;
		
		GUI.Label(new Rect(600,40,100,50), time, style1);
		GUI.Label(new Rect(600,60,100,50), score, style1);
		GUI.Label(new Rect(600,80,100,50), combo, style1);
		GUI.Label(new Rect(600,100,100,50), evolution, style1);	
		GUI.Label(new Rect(600,120,100,50), healt, style1);
	}
}
