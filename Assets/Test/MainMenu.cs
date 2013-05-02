using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	private const int buttonWidth = 100;
	private const int buttonHeight = 50;
	private const int buttonDistance = 10;
	
	private int centerScreen;
	private int startTopPosition;
	
	private int newGameTop;
	private int scoreTop;
	private int optionsTop;
	private int creditsTop;
	
	// Use this for initialization
	void Start ()
	{
		centerScreen = Screen.width / 2 - buttonWidth / 2;		
		
		newGameTop = 0;
		scoreTop = buttonHeight + buttonDistance;
		optionsTop = (buttonHeight + buttonDistance) * 2;
		creditsTop = (buttonHeight + buttonDistance) * 3;
		
		startTopPosition = Screen.height / 2 - (creditsTop + buttonHeight + buttonDistance) / 2;
		
		newGameTop += startTopPosition;
		scoreTop += startTopPosition;
		optionsTop += startTopPosition;
		creditsTop += startTopPosition;
	}
	
	// Update is called once per frame
	void Update () 
	{	
	}
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(centerScreen, newGameTop, buttonWidth, buttonHeight), "New Game"))
            Application.LoadLevel("LoadingScene");
		if (GUI.Button(new Rect(centerScreen, scoreTop, buttonWidth, buttonHeight), "Top Score"))
            Debug.Log("Clicked the button with text");
		if (GUI.Button(new Rect(centerScreen, optionsTop, buttonWidth, buttonHeight), "Options"))
            Debug.Log("Clicked the button with text");
		if (GUI.Button(new Rect(centerScreen, creditsTop, buttonWidth, buttonHeight), "Credits"))
            Debug.Log("Clicked the button with text");
	}	
}
