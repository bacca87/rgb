using UnityEngine;
using System.Collections;

public class LegacyGui : MonoBehaviour {
	
	public bool isVisible;
	public bool quitButtonActive;
	public ScoreoidManager scoreoidManager;
	public string usernameField = "username";
	public string passwordField = "password";
	public string responseMessage = "messaggio di risposta";
	
	void Start()
	{
		isVisible = false;
		quitButtonActive = false;
		if (PlayerPrefs.HasKey("Username"))
			usernameField = PlayerPrefs.GetString("Username");
		if (PlayerPrefs.HasKey("Password"))
			passwordField = PlayerPrefs.GetString("Password");
	}
	
	// Use this for initialization
	void OnGUI () 
	{
		if (isVisible) 
		{
			// Box contenitore di sfondo
			GUI.Box (new Rect (0,0,Screen.width,Screen.height), ""); // per rendere grigio lo sfondo.
			
			// Gruppo centrale
			GUI.BeginGroup (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200));
			// Box di dialogo
			GUI.Box (new Rect (0,0, 200, 200), "Scoreoid Login");
			// Username
			GUI.Label (new Rect (10, 25, 90, 20), "Username");
			usernameField = GUI.TextField (new Rect (100, 25, 90, 20), usernameField, 10);
			// Password
			GUI.Label (new Rect (10, 50, 90, 20), "Password");
			passwordField = GUI.PasswordField(new Rect (100, 50, 90, 20), passwordField, "*"[0], 10);
			// Bottoni registra/login
			if (GUI.Button (new Rect (10, 75, 87, 40), "Login")) 
			{
				print ("LOGIN PRESSED");
//				StartCoroutine(loginFunction());
				loginFunction();
			}
			if (GUI.Button (new Rect (106, 75, 87, 40), "Register")) 
			{
				print ("REGISTER PRESSED");
//				StartCoroutine(registerFunction());
				registerFunction();
			}
			// Messaggio di risposta
			GUI.Label (new Rect (10, 120, 180, 35), responseMessage);
			// Bottone uscita
			if (GUI.Button (new Rect (10, 155, 180, 40), "Quit")) 
			{
				print ("QUIT PRESSED");
				StartCoroutine(onQuitButtonPressed());
			}
			GUI.EndGroup();
		}
	}
	
	private IEnumerator onQuitButtonPressed()
	{
		isVisible = false;
		yield return new WaitForSeconds(0.5f);
		quitButtonActive = false;
	}
	
	private void loginFunction()
	{
		print ("LOGIN FUNCTION");
		// Setta valori username e password su scoreoidMAnager
		scoreoidManager.username = usernameField;
		scoreoidManager.password = passwordField;
		// Lancia createUser da scoreoidManager e aspetta che ritorni
		StartCoroutine(scoreoidManager.downloadUserData());
	}
	
	private void registerFunction()
	{	print ("REGISTER FUNCTION");
		// Setta valori username e password su scoreoidMAnager
		scoreoidManager.username = usernameField;
		scoreoidManager.password = passwordField;
		// Lancia createUser da scoreoidManager e aspetta che ritorni
		StartCoroutine(scoreoidManager.createUser());
	}
}
