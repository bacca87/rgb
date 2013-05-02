using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PageOLD
{
	Main = 1,
	Options = 2,
	Leaderboard = 3,
	Credits = 4
}

[System.Serializable]
public class UIMainMenuOLD : MonoBehaviour 
{
	public ScoreoidManager scoreoidManager;
	public LegacyGui legacyGui;
	
	PageOLD currentPage;
	UISprite background;
	
	// Main Menu vars
	UIButton playButton = null;
	UIButton optionsButton;
	UIButton leaderboardButton;
	UIButton creditsButton;
	
	// Options vars
	UISprite audioLabel = null;
	UIToggleButton audioCheckBox;
	UISprite sensitivityLabel;
	UISlider sensitivitySlider; 
	UIButton accountButton;
	
	// Leaderboard Vars
	UIButton friendsButton = null;
	UIButton globalButton;
	UIText leaderboardTextManager;
	UITextInstance leaderboardText = null;
	UIButton shareButton;
	
	// Credits Vars
	UIText creditsTextManager = null;
	UITextInstance creditsText;
	
	// Common vars
	UIButton quitButton = null;
	UIButton backButton = null;

	// Use this for initialization
	void Start () 
	{	
		// Background
//		background = UI.firstToolkit.addSprite("background.png", 0, 0);
		
		// Codice per il debug
		if (!PlayerPrefs.HasKey("FriendsLeaderboard"))
				PlayerPrefs.SetString("FriendsLeaderboard", "Friends Leaderboard\n1.asd\n2.asd");
		if (!PlayerPrefs.HasKey("GlobalLeaderboard"))
				PlayerPrefs.SetString("GlobalLeaderboard", "Global Leaderboard\n1.asd\n2.asd");
		// Fine codice per il debug
		
		// Setup dello scoreoid manager con valori salvati in locale
		if (PlayerPrefs.HasKey("Username"))
			scoreoidManager.username = PlayerPrefs.GetString("Username");
		if (PlayerPrefs.HasKey("Password"))
			scoreoidManager.password = PlayerPrefs.GetString("Password");
		if (PlayerPrefs.HasKey("BestScore"))
			scoreoidManager.bestScore = PlayerPrefs.GetInt("BestScore");
		if (PlayerPrefs.HasKey("TimePlayed"))
			scoreoidManager.timePlayed = PlayerPrefs.GetInt("TimePlayed");
		if (PlayerPrefs.HasKey("Rank"))
			scoreoidManager.rank = PlayerPrefs.GetInt("Rank");
		
		legacyGui.isVisible = false;
		
		// Aggiornamento dei dati locali
		StartCoroutine(updateUserData());
		StartCoroutine(updateLeaderboards());
		
		StartCoroutine(createMainMenu());
		StartCoroutine(createQuitButton());
	}
	
	void OnGui()
	{
//		if(true)
//		{
			// Background box
			GUI.Box(new Rect(Screen.width/2,Screen.height/2,100,80), "RGB Account");
//		}
	}
	
//********** BUTTON EVENT HANDLERS **********//
	
	private void onTouchPlayButton(UIButton sender)
	{
		Debug.Log("Bottone PLAY premuto");
		StartCoroutine(delayedLaunch());
	}
	
	private void onTouchOptionsButton(UIButton sender)
	{		
		Debug.Log("Bottone OPTIONS premuto");
		StartCoroutine(removeMainMenu());
		StartCoroutine(removeQuitButton());
		StartCoroutine(createOptionsMenu());
		StartCoroutine(createBackButton());
	}
 
	private void onTouchLeaderboardButton(UIButton sender)
	{
		Debug.Log("Bottone LEADERBOARD premuto");
		StartCoroutine(removeMainMenu());
		StartCoroutine(removeQuitButton());
		StartCoroutine(createLeaderboardMenu());
		StartCoroutine(createBackButton());
	}
	
	private void onTouchCreditsButton(UIButton sender)
	{
		Debug.Log("Bottone CREDITS premuto");
		StartCoroutine(removeMainMenu());
		StartCoroutine(removeQuitButton());
		StartCoroutine(createCreditsMenu());
		StartCoroutine(createBackButton());
	}
	
	private void onTouchQuitButton(UIButton sender)
	{
		Debug.Log("Bottone QUIT premuto");
		Application.Quit();
	}
	
	private void onTouchBackButton(UIButton sender)
	{
		Debug.Log("Bottone BACK premuto");
		switch (currentPage)
		{
		case PageOLD.Options:
			StartCoroutine(removeOptionsMenu());
			StartCoroutine(removeBackButton());
			StartCoroutine(createMainMenu());
			StartCoroutine(createQuitButton());
			break;
		case PageOLD.Leaderboard:
			StartCoroutine(removeLeaderboardMenu());
			StartCoroutine(removeBackButton());
			StartCoroutine(createMainMenu());
			StartCoroutine(createQuitButton());
			break;
		case PageOLD.Credits:
			StartCoroutine(removeCreditsMenu());
			StartCoroutine(removeBackButton());
			StartCoroutine(createMainMenu());
			StartCoroutine(createQuitButton());
			break;
		}
	}
	
	private void onTouchGlobalButton(UIButton sender)
	{
		// Componi stringa con valori di scoreoid (se ci sono) e metti sui playerprefs
		if (scoreoidManager.leaderboard.Count != 0)
		{
			string leaderboardString = "Global Leaderboard\n";
			int index = 1;
			foreach (KeyValuePair<string, int> player in scoreoidManager.leaderboard)
			{	
				leaderboardString += index + "." + player.Key + "     " + player.Value + "\n";
				index++;
			}
			PlayerPrefs.SetString("GlobalLeaderboard", leaderboardString);
		}
		// Visualizza dai playerprefs
		if (PlayerPrefs.HasKey("GlobalLeaderboard"))
			leaderboardText.text = PlayerPrefs.GetString("GlobalLeaderboard");
	}
	
	private void onTouchFriendsButton(UIButton sender)
	{
		// Componi stringa con valori di scoreoid (se ci sono) e metti sui playerprefs
		if (scoreoidManager.friendsLeaderboard.Count != 0)
		{
			string leaderboardString = "Friends Leaderboard\n";
			int index = 1;
			foreach (KeyValuePair<string, int> player in scoreoidManager.friendsLeaderboard)
			{	
				leaderboardString += index + "." + player.Key + "     " + player.Value + "\n";
				index++;
			}
			PlayerPrefs.SetString("FriendsLeaderboard", leaderboardString);
		}
		if (PlayerPrefs.HasKey("FriendsLeaderboard"))
			leaderboardText.text = PlayerPrefs.GetString("FriendsLeaderboard");
	}
	
	private void onTouchShareButton(UIButton sender)
	{
		// TODO DOPO LA BETA
	}
	
	private void onToggleAudioButton(UIToggleButton sender, bool selected)
	{
		if(selected)
			PlayerPrefs.SetInt("Audio", 0);
			// TODO: Disattiva una eventuale musica
		else 
			PlayerPrefs.SetInt("Audio", 1);
			// TODO: Riattiva una eventuale musica
	}
	
	private void onSensitivitySliderChanged(UISlider sender, float value)
	{
		PlayerPrefs.SetFloat("Sensitivity", value);
	}
	
	private void onTouchAccountButton(UIButton sender)
	{
		if(!legacyGui.isVisible && !legacyGui.quitButtonActive)
		{
			legacyGui.isVisible = true;
			legacyGui.quitButtonActive = true;
		}
	}
	
	//********** COROUTINES DI GRAFICA **********//
		
	private IEnumerator createMainMenu()
	{
		Debug.Log("Create MainMenu");
		currentPage = PageOLD.Main;
		if (playButton == null)
		{
			// Aggiungere sprite con logo/scritta RGB grande
			
			// Play button
			playButton = UIButton.create( "RGB_button.png", "RGB_button.png", 0, 0 );
			playButton.onTouchUp += onTouchPlayButton;
			
			// Options button
			optionsButton = UIButton.create( "RGB_button.png", "RGB_button.png", 0, 0 );
			optionsButton.parentUIObject = playButton;
			optionsButton.onTouchUp += onTouchOptionsButton;
			
			// Leaderboard button
			leaderboardButton = UIButton.create( "RGB_button.png", "RGB_button.png", 0, 0 );
			leaderboardButton.parentUIObject = playButton;
			leaderboardButton.onTouchUp += onTouchLeaderboardButton;
			
			// Credits button
			creditsButton = UIButton.create( "RGB_button.png", "RGB_button.png", 0, 0 );
			creditsButton.parentUIObject = playButton;
			creditsButton.onTouchUp += onTouchCreditsButton;
		}
		
		// Posizioni
		playButton.positionFromTop(0.05f);
		optionsButton.positionFromBottom(-1.1f);
		leaderboardButton.positionFromBottom(-2.2f);
		creditsButton.positionFromBottom(-3.3f);
		
		// Animazione
		UIObjectAnimationExtensions.positionFrom(playButton, 1.0f, new Vector3(-Screen.width, playButton.position.y, playButton.position.z), Easing.Quartic.easeIn);
		yield return null;
	}
	
	private IEnumerator removeMainMenu()
	{
		Debug.Log("Remove MainMenu");
		playButton.positionTo(1.0f, new Vector3(playButton.position.x-Screen.width, playButton.position.y, playButton.position.z), Easing.Quartic.easeIn);
		yield return new WaitForSeconds(1);
//		playButton.destroy();
//		optionsButton.destroy();
//		leaderboardButton.destroy();
//		creditsButton.destroy();
	}
	
	private IEnumerator createQuitButton()
	{
		// Quit button
		if (quitButton == null)
		{
			quitButton = UIButton.create( "RGB_slider_thumb.png", "RGB_slider_thumb.png", 0, 0 );
			quitButton.onTouchUp += onTouchQuitButton;
		}
		quitButton.positionFromBottomRight(0.01f, 0.01f);
		quitButton.positionFrom(1.0f, new Vector3(Screen.width * 2, quitButton.position.y, quitButton.position.z), Easing.Quartic.easeIn);
		yield return null;
	}
	
	private IEnumerator removeQuitButton()
	{
		quitButton.positionTo(1.0f, new Vector3(Screen.width * 2, quitButton.position.y, quitButton.position.z), Easing.Quartic.easeIn);
//		quitButton.destroy();
		yield return null;
	}
	
	private IEnumerator createBackButton()
	{
		// Back button
		if (backButton == null)
		{
			backButton = UIButton.create( "RGB_slider_thumb.png", "RGB_slider_thumb.png", 0, 0 );
			backButton.onTouchUp += onTouchBackButton;
		}
		backButton.positionFromBottomRight(0.01f, 0.01f);
		backButton.positionFrom(1.0f, new Vector3(Screen.width * 2, backButton.position.y, backButton.position.z), Easing.Quartic.easeIn);
		yield return null;
	}
	
	private IEnumerator removeBackButton()
	{
		backButton.positionTo(1.0f, new Vector3(Screen.width * 2, backButton.position.y, backButton.position.z), Easing.Quartic.easeIn);
//		backButton.destroy();
		yield return null;
	}
	
	private IEnumerator createOptionsMenu()
	{
		Debug.Log("Create OptionsMenu");
		currentPage = PageOLD.Options;
		if(audioLabel == null)
		{
			// Audio Label
			audioLabel = UI.firstToolkit.addSprite("RGB_button.png", 0, 0);
			
			// Audio Checkbox
			audioCheckBox = UIToggleButton.create("RGB_slider_thumb.png", "RGB_slider_thumb.png","RGB_slider_thumb.png", 0, 0);
			audioCheckBox.parentUIObject = audioLabel;
			audioCheckBox.onToggle += onToggleAudioButton;
			
			// Sens Label
			sensitivityLabel = UI.firstToolkit.addSprite("RGB_button.png", 0, 0);
			sensitivityLabel.parentUIObject = audioLabel;
			
			// Sens Slider
			sensitivitySlider = UISlider.create("RGB_slider_thumb.png", "RGB_slider_bar.png", 0, 0,UISliderLayout.Horizontal, 1, true);
			if(PlayerPrefs.HasKey("Sensitivity"))
				sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
			sensitivitySlider.onChange += onSensitivitySliderChanged;
			
			// Account button
			accountButton = UIButton.create("RGB_button.png", "RGB_button.png", 0, 0); 
			accountButton.parentUIObject = audioLabel;
			accountButton.onTouchUp += onTouchAccountButton;
		}
					
		// Posizioni
		audioLabel.positionFromTop(0.05f);
		audioCheckBox.positionFromRight(-0.5f);
		sensitivityLabel.positionFromBottom(-1.1f);
		sensitivitySlider.position = sensitivityLabel.position + new Vector3(sensitivityLabel.width, -sensitivityLabel.height/2, 0.0f);
		accountButton.positionFromBottom(-2.2f);
		
		// Animazione
		audioLabel.positionFrom(1.0f, new Vector3(-Screen.width, audioLabel.position.y, audioLabel.position.z), Easing.Quartic.easeIn);
		sensitivitySlider.positionFrom(1.0f, new Vector3(-Screen.width, sensitivitySlider.position.y, sensitivitySlider.position.z), Easing.Quartic.easeIn);
		yield return null;
	}
	
	private IEnumerator removeOptionsMenu()
	{
		Debug.Log("Remove OptionsMenu");
		audioLabel.positionTo(1.0f, new Vector3(audioLabel.position.x-Screen.width, audioLabel.position.y, audioLabel.position.z), Easing.Quartic.easeIn);
		sensitivitySlider.positionTo(1.0f, new Vector3(sensitivitySlider.position.x-Screen.width, sensitivitySlider.position.y, sensitivitySlider.position.z), Easing.Quartic.easeIn);
		yield return new WaitForSeconds(1);
	}
	
	private IEnumerator createLeaderboardMenu()
	{
		Debug.Log("Create OptionsMenu");
		currentPage = PageOLD.Leaderboard;
		if(friendsButton == null)
		{
			friendsButton = UIButton.create("RGB_button.png", "RGB_button.png", 0, 0); 
			friendsButton.onTouchUp += onTouchFriendsButton;
			globalButton = UIButton.create("RGB_button.png", "RGB_button.png", 0, 0); 
			globalButton.parentUIObject = friendsButton;
			globalButton.onTouchUp += onTouchGlobalButton;
			leaderboardTextManager = new UIText("font", "font.png");
			if (PlayerPrefs.HasKey("FriendsLeaderboard"))
				leaderboardText = leaderboardTextManager.addTextInstance(PlayerPrefs.GetString("FriendsLeaderboard"), 0, 0, 1.0f);
			else
				leaderboardText = leaderboardTextManager.addTextInstance("Couldn't find a friends leaderboard.\n" +
												   	   "Try to log in from the options menu.", 0, 0);
			leaderboardText.parentUIObject = friendsButton;
			shareButton = UIButton.create("RGB_button.png", "RGB_button.png", 0, 0); 
			shareButton.onTouchUp += onTouchShareButton;			
		}
		
		// Posizioni
		friendsButton.positionFromTopLeft(0.05f, 0.05f);
		globalButton.positionFromLeft(1.05f); // rispetto a friendsbutton
		leaderboardText.positionFromTopLeft(1.0f, 0.0f); // rispetto a friendsbutton
		shareButton.positionFromBottomLeft(0.05f, 0.05f);
		
		// Animazione
		friendsButton.positionFrom(1.0f, new Vector3(-Screen.width, friendsButton.position.y, friendsButton.position.z), Easing.Quartic.easeIn);
		shareButton.positionFrom(1.0f, new Vector3(-Screen.width, shareButton.position.y, shareButton.position.z), Easing.Quartic.easeIn);
		yield return null;
	}
	
	private IEnumerator removeLeaderboardMenu()
	{
		Debug.Log("Remove LeaderboardMenu");
		friendsButton.positionTo(1.0f, new Vector3(friendsButton.position.x-Screen.width, friendsButton.position.y, friendsButton.position.z), Easing.Quartic.easeIn);
		shareButton.positionTo(1.0f, new Vector3(shareButton.position.x-Screen.width, shareButton.position.y, shareButton.position.z), Easing.Quartic.easeIn);
		yield return null;
	}
	
	private IEnumerator createCreditsMenu()
	{		
		Debug.Log("Create OptionsMenu");
		currentPage = PageOLD.Credits;
		if(creditsText == null)
		{
			creditsTextManager = new UIText("font", "font.png");
			creditsText = creditsTextManager.addTextInstance("Credits, Mamba pro Bacca gay everyday \n nigger nigger nigger", 0, 0);
		}
		yield return null;
		
		// Posizioni
		creditsText.positionCenter();
		
		// Animazione
		creditsText.positionFrom(1.0f, new Vector3(-Screen.width, creditsText.position.y, creditsText.position.z), Easing.Quartic.easeIn); 
	}
	
	private IEnumerator removeCreditsMenu()
	{
		Debug.Log("Remove CreditsMenu");
		creditsText.positionTo(1.0f, new Vector3(creditsText.position.x-Screen.width, creditsText.position.y, creditsText.position.z), Easing.Quartic.easeIn);
		yield return null;
	}
	
	//********** COROUTINES DI DATI **********//
	
	private IEnumerator updateUserData()
	{
		// TODO: Controlla la connessione?
		
		if ( PlayerPrefs.HasKey("Username") && PlayerPrefs.HasKey("Password") )
		{
			scoreoidManager.downloadUserData();
			PlayerPrefs.SetInt("BestScore", scoreoidManager.bestScore);
			PlayerPrefs.SetInt("TimePlayed", scoreoidManager.timePlayed);
			PlayerPrefs.SetInt("Rank", scoreoidManager.rank);
		}
		else
		{
			// TODO: SPARA MESSAGGIO ONGUI  che chiede se si vuole creare un account -> se si presenta il form per crearlo	
		}
		yield return null;
	}
	
	private IEnumerator updateLeaderboards()
	{
		// TODO: Controlla la connessione?
		
		// Update leaderboard data
		scoreoidManager.downloadBestScores(10);
		scoreoidManager.downloadFriendsScores();
		
		// TODO: poossibilmente aspettare che finisca?
		yield return null;
	}
	
	private IEnumerator delayedLaunch()
	{
		Debug.Log("Delayed launch...");
		yield return new WaitForSeconds(1);
		Debug.Log("Launched!");
		Application.LoadLevel("LoadingGame");
	}
}
