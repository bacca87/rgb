using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Scene
{
	MainMenu,
	Loading,
	Game,
	Score
}

[System.Serializable]
public class UIMainMenu : MonoBehaviour 
{
	public GameObject creditsPrefab;		
	public GameObject loadingEffect;
	public float waitTime = 1;
	public Scene currentScene = Scene.MainMenu;
	
	UIButton playButton;
	UIButton infoButton;	
	UIButton quitButton;
	UIButton backButton;
	UIButton homeButton;
	UIButton leaderboardButton;
	UIButton pauseButton;
	UIButton swarmButton;
	UIButton splashScreenButton;
	UIToggleButton audioButton;
	
	UIProgressBar evolutionBar;
		
	UISprite comboLabel;
	UISprite background;
	UISprite finalScoreBox;
	UISprite creditsLogo;
	
	UIScrollableVerticalLayout creditsScrollableContainer;
	
	UIText textManager;	
	UITextInstance infoText;
	UITextInstance splashText;
	UITextInstance loadingText;
	UITextInstance rgbMasterText;
	UITextInstance scoreText;
	UITextInstance finalScoreText;
	UITextInstance totalESCatchedText;
	UITextInstance totalESMissedText;
	UITextInstance totalESSpawnedText;
	UITextInstance totalGameTimeText;
	UITextInstance totalComboText;
	UITextInstance totalEvolutionsText;
		
	GameManager gameManager;	
	bool isPaused;
	bool showHUD = true;
		
	static bool runOnce = true;
	
	RGBSocial social;
	GameObject creditsObject;
	
	// Use this for initialization
	void Start ()
	{
		// Background
//		background = UI.firstToolkit.addSprite("background.png", 0, 0);
		
		social = (RGBSocial)FindObjectOfType(typeof(RGBSocial));
		
		textManager = new UIText("font", "font.png");
		
		isPaused = false;
				
		switch (currentScene)
		{
		case Scene.MainMenu:
			if(runOnce)
			{
				createSplashScreen();
				runOnce = false;
			}
			else
				createMainMenuPage();
			break;
		case Scene.Loading:
			createLoadingScreen();
			break;
		case Scene.Game:
			showHUD = true;
			gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
			createGameHUD();
			break;
		}
	}
	
	void Update()
	{
		if(currentScene == Scene.Game)
		{
			if(gameManager.showScore)
			{
				gameManager.showScore = showHUD = false;
				removeGameHUD();
				createStatsPanel();
			}
			
			if(showHUD && gameManager.updateHUD)
			{
				gameManager.updateHUD = false;
					
				evolutionBar.value = (float)gameManager.evolutionRate / 100;
				scoreText.text = "score: " + gameManager.scorePoints.ToString();
				scoreText.pixelsFromLeft(-(int)scoreText.width - 5);
				
				if(gameManager.comboPoints == gameManager.combo.x2)
				{
					comboLabel.setSpriteImage("comboX1.png");
					comboLabel.pixelsFromBottomRight(15, 15);
				}
				if(gameManager.comboPoints == gameManager.combo.x3)
				{
					comboLabel.setSpriteImage("comboX2.png");
					comboLabel.pixelsFromBottomRight(15, 15);
				}
				if(gameManager.comboPoints == gameManager.combo.x4)
				{
					comboLabel.setSpriteImage("comboX3.png");
					comboLabel.pixelsFromBottomRight(15, 15);
				}
			}
		}
	}
	
	void onTouchSplashScreen(UIButton sender)
	{
		Debug.Log("Splash Screen premuto");
		removeSplashScreen();
		createMainMenuPage();
	}
	
	void onTouchPlayButton(UIButton sender)
	{
		Debug.Log("Bottone PLAY premuto");
		removeMainMenuPage();
		removeAudioButton();
		createLoadingScreen();
		
		social.save();
		
		StartCoroutine(loadGame());
	}
	
	IEnumerator loadGame()
	{
		yield return new WaitForSeconds(waitTime);
		Application.LoadLevelAdditive("GameScene");
		Destroy(transform.parent.gameObject); //distrugge l'oggetto padre della scena eliminando tutto al caricamento del gioco
	}
		
	void onTouchInfoButton(UIButton sender)
	{
		Debug.Log("Bottone info premuto");
		removeMainMenuPage();
		createInfoPage();
		social.checkInfoRead();
	}
	
	void onTouchQuitButton(UIButton sender)
	{
		Debug.Log("Bottone QUIT premuto");
		social.save();
		Application.Quit();
	}
	
	void onTouchBackButton(UIButton sender)
	{
		Debug.Log("Bottone BACK premuto");
		
		removeInfoPage();
		createMainMenuPage();
	}
	
	void onToggleAudioButton( UIToggleButton sender, bool selected )
	{
		AudioListener.pause = selected;
	}
	
	void onTouchSwarmButton(UIButton sender)
	{
		Swarm.showDashboard();	
	}
	
	void onTouchLeaderboardButton(UIButton sender)
	{
		Swarm.showLeaderboards();
	}
	
	void onTouchPauseButton(UIButton sender)
	{			
		if(!isPaused)
		{
			isPaused = true;
	    	GameObject[] gos = GameObject.FindGameObjectsWithTag("Pausable");
	    	foreach(GameObject go in gos)
				go.SendMessage("Pause");
				
			homeButton = UIButton.create( "home.png", "home.png", 0, 0 );
			homeButton.onTouchUp += onTouchHomeButton;
			homeButton.parentUIObject = infoButton;
			homeButton.positionFromTopLeft(-1.0f, 0.0f);
			
			swarmButton = UIButton.create( "swarm_icon.png", "swarm_icon.png", 0, 0 );
			swarmButton.onTouchUp += onTouchSwarmButton;
			swarmButton.parentUIObject = homeButton;
			swarmButton.positionFromTopLeft(-1.0f, 0.0f);
		}
		else
		{
			isPaused = false;
			GameObject[] gos = GameObject.FindGameObjectsWithTag("Pausable");
	    	foreach(GameObject go in gos)
				go.SendMessage("Resume");
			homeButton.destroy();
			swarmButton.destroy();
		}
	}
		
	void onTouchResumeButton(UIButton sender)
	{
		homeButton.destroy();
		swarmButton.destroy();
	}
	
	void onTouchHomeButton(UIButton sender)
	{
		Application.LoadLevel("MainMenuScene");
	}
	
	void createMainMenuPage()
	{
		Debug.Log("Create MainMenu");

		// Aggiungere sprite con logo/scritta RGB grande
		
		createAudioButton();
		
		// Play button
		playButton = UIButton.create( "play.png", "play.png", 0, 0 );
		playButton.onTouchUp += onTouchPlayButton;
		playButton.positionCenter();
		
		// info button
		infoButton = UIButton.create( "info.png", "info.png", 0, 0 );
		infoButton.onTouchUp += onTouchInfoButton;
		infoButton.positionFromBottomLeft(0.01f, 0.01f);
		
		// Swarm button
		swarmButton = UIButton.create( "swarm_icon.png", "swarm_icon.png", 0, 0 );
		swarmButton.onTouchUp += onTouchSwarmButton;
		swarmButton.positionFromTopRight(0.01f, 0.01f);
		
		// Quit button
		quitButton = UIButton.create( "quit.png", "quit.png", 0, 0 );
		quitButton.onTouchUp += onTouchQuitButton;
		quitButton.positionFromBottomRight(0.01f, 0.01f);
		
		// Animazione
		//UIObjectAnimationExtensions.positionFrom(playButton, 1.0f, new Vector3(-Screen.width, playButton.position.y, playButton.position.z), Easing.Quartic.easeIn);
	}
	
	void removeMainMenuPage()
	{
		Debug.Log("Remove MainMenu");
		playButton.destroy();
		infoButton.destroy();
		swarmButton.destroy();
		quitButton.destroy();		
	}
	
	void createAudioButton()
	{
		audioButton = UIToggleButton.create( "audio_on.png", "audio_off.png", "audio_on.png", 0, 0 );
		audioButton.onToggle += onToggleAudioButton;
		audioButton.selected = AudioListener.pause;
		audioButton.positionFromTopLeft(0.01f, 0.01f);
	}
	
	void removeAudioButton()
	{
		Debug.Log("Remove audioButton");
		audioButton.destroy();
	}
	
	void createInfoPage()
	{		
		Debug.Log("Create OptionsMenu");
		
		// Back button
		backButton = UIButton.create( "back.png", "back.png", 0, 0 );
		backButton.onTouchUp += onTouchBackButton;	
		backButton.positionFromBottomLeft(0.01f, 0.01f);
		/*
		creditsScrollableContainer = new UIScrollableVerticalLayout(0);
//		creditsScrollableContainer.alignMode = UIAbstractContainer.UIContainerAlignMode.Center;
		creditsScrollableContainer.setSize( Screen.width * 0.5f, Screen.height * 3.0f );
		creditsScrollableContainer.positionFromTopLeft(0.0f, 0.25f);
		creditsLogo = UI.firstToolkit.addSprite("splash.png", 0, 0);
		creditsLogo.positionFromTop(0.0f);
		infoText = textManager.addTextInstance("RGB Copyright 2013 NOME GRUPPO\n" +
			"VERSION 1.0\n" +
			"In alphabetical order:\n" +
			"\nDesigners, Developers, Marketing and so on...\n" +
			"Alessandro Maroso\n" +
			"Marco Baccarani\n" +
			"\n3D Modeler\n" +
			"Marco Giacobbo\n" +
			"\n2D Artist\n" +
			"nigger\n" +
			"\nSound Designer & Music\n" +
			"nigger\n" +
			"\nMarketing & PR\n" +
			"Alessandro Maroso\n" +
			"Marco Baccarani\n" +
			"\nTesters\n" +
			"Luca Verrascina\n" +
			"\nSpecial Greetings To\n" +
			"Our families\n" +
			"UNITY3D\n" +
			"The fans of RGB\n"
			, 0, 0);
		UISpacer spacer1= new UISpacer(Screen.width/2, Screen.height);
		UISpacer spacer2 = new UISpacer(Screen.width/2, Screen.height);
		UISpacer spacer3 = new UISpacer(Screen.width/2, Screen.height);
		UISpacer spacer4 = new UISpacer(Screen.width/2, Screen.height);
		infoText.parentUIObject = spacer1;
		infoText.positionFromBottom(-1.0f);
		infoText.alignMode = UITextAlignMode.Center;
		
		creditsScrollableContainer.addChild(creditsLogo, spacer1, spacer2, spacer3, spacer4);
		
		// Animazione
		//infoText.positionFrom(1.0f, new Vector3(-Screen.width, infoText.position.y, infoText.position.z), Easing.Quartic.easeIn); 
		*/
		
		creditsObject = Instantiate(creditsPrefab, Vector3.zero, Quaternion.identity) as GameObject;
	}
	
	void removeInfoPage()
	{
		Debug.Log("Remove InfoMenu");
		//creditsLogo.destroy();
		backButton.destroy();
		Destroy(creditsObject);
		//infoText.destroy();
		//creditsScrollableContainer.Clear();
	}
	
	void createSplashScreen()
	{
		Debug.Log("Create Splash Screen");
		
		splashScreenButton = UIButton.create( "splash.png", "splash.png", 0, 0 );
		splashScreenButton.onTouchUp += onTouchSplashScreen;
		splashScreenButton.positionCenter();
				
		splashText = textManager.addTextInstance("Tap to continue...", 0, 0);
		splashText.positionFromBottom(0.1f);
	}
	
	void removeSplashScreen()
	{
		Debug.Log("Remove Splash Screen");		
		splashScreenButton.destroy();
		splashText.destroy();
	}
	
	void createLoadingScreen()
	{
		Debug.Log("Create Loading Screen");
		
		//rgbMasterText = infoTextManager.addTextInstance("Bacca is now the RGB master!!!", 0, 0);
		//rgbMasterText.positionCenter();
		
		loadingText = textManager.addTextInstance("Loading...", 0, 0);
		//loadingText.positionFromBottomRight(0.01f, 0.01f);
		loadingText.positionCenter();
		
		//istanzio la evo che ruota per l'effetto caricamento
		GameObject loadingObj = Instantiate(loadingEffect, Vector3.zero, Quaternion.identity) as GameObject;		
		loadingObj.transform.parent = transform.parent; //lo metto figlio dell'oggetto da distruggere al caricamento
		loadingObj.transform.position = new Vector3(4.5f,0.15f,0);
	}
	
	public void removeLoadingScreen()
	{
		Debug.Log("remove Loading Screen");
		loadingText.destroy();
	}
	
	void createGameHUD()
	{
		createAudioButton();
		
		// pause button
		infoButton = UIButton.create( "pause.png", "pause.png", 0, 0 );
		infoButton.onTouchUp += onTouchPauseButton;
		infoButton.positionFromBottomLeft(0.01f, 0.01f);
		
		// evolutionbar
		evolutionBar = UIProgressBar.create("evolutionBar.png", 0, 0);
		evolutionBar.pixelsFromTopRight(15, 335);
		evolutionBar.value = 1.0f;
		evolutionBar.resizeTextureOnChange = true;
		
		// score
		scoreText = textManager.addTextInstance("score: 0", 0, 0);
		scoreText.parentUIObject = evolutionBar;
		scoreText.pixelsFromLeft(-(int)scoreText.width - 5);
		
		// combo label
		comboLabel = UI.firstToolkit.addSprite("comboX0.png", 0, 0);
		comboLabel.pixelsFromBottomRight(15, 15);		
	}
	
	void removeGameHUD()
	{
		infoButton.destroy();
		evolutionBar.destroy();
		scoreText.destroy();
		comboLabel.destroy();
		if (homeButton != null)
			homeButton.destroy();
		if (swarmButton != null)
			swarmButton.destroy();
	}
	
	void createStatsPanel()
	{
		finalScoreBox = UI.firstToolkit.addSprite("finalScoreBox.png", 0, 0);
		finalScoreBox.positionCenter();
		
		string finalScore = "score: " + gameManager.scorePoints.ToString();
		finalScoreText = textManager.addTextInstance(finalScore, 0.0f, 0.0f, 1.5f);
		finalScoreText.parentUIObject = finalScoreBox;
		finalScoreText.positionFromTopLeft(0.02f, 0.02f);
		
		string totalESCatched = "total ES catched: " + gameManager.totalESCatched.ToString();
		totalESCatchedText = textManager.addTextInstance(totalESCatched, 0.0f, 0.0f, 1.0f);
		totalESCatchedText.parentUIObject = finalScoreText;
		totalESCatchedText.positionFromTopLeft(1.01f, 0.0f);
		
		string totalESMissed = "total ES missed: " + gameManager.totalESMissed.ToString();
		totalESMissedText = textManager.addTextInstance(totalESMissed, 0.0f, 0.0f, 1.0f);
		totalESMissedText.parentUIObject = totalESCatchedText;
		totalESMissedText.positionFromTopLeft(1.01f, 0.0f);
		
		string totalESSpawned = "total ES spawned: " + gameManager.totalESSpawned.ToString();
		totalESSpawnedText = textManager.addTextInstance(totalESSpawned, 0.0f, 0.0f, 1.0f);
		totalESSpawnedText.parentUIObject = totalESMissedText;
		totalESSpawnedText.positionFromTopLeft(1.01f, 0.0f);
		
		string totalGameTime = "game time: " + gameManager.totalGameTime.ToString();
		totalGameTimeText = textManager.addTextInstance(totalGameTime, 0.0f, 0.0f, 1.0f);
		totalGameTimeText.parentUIObject = totalESSpawnedText;
		totalGameTimeText.positionFromTopLeft(1.01f, 0.0f);
		
		string totalCombo = "combos number: " + gameManager.totalCombo.ToString();
		totalComboText = textManager.addTextInstance(totalCombo, 0.0f, 0.0f, 1.0f);
		totalComboText.parentUIObject = totalGameTimeText;
		totalComboText.positionFromTopLeft(1.01f, 0.0f);
		
		string totalEvolutions = "evolution level: " + gameManager.totalEvolutions.ToString();
		totalEvolutionsText = textManager.addTextInstance(totalEvolutions, 0.0f, 0.0f, 1.0f);
		totalEvolutionsText.parentUIObject = totalComboText;
		totalEvolutionsText.positionFromTopLeft(1.01f, 0.0f);
		
		leaderboardButton = UIButton.create( "leaderboard.png", "leaderboard.png", 0, 0 );
		leaderboardButton.onTouchUp += onTouchLeaderboardButton;;
		leaderboardButton.parentUIObject = finalScoreBox;
		leaderboardButton.positionFromBottomRight(0.01f, 0.01f);
		
		homeButton = UIButton.create( "home.png", "home.png", 0, 0 );
		homeButton.onTouchUp += onTouchHomeButton;
		homeButton.parentUIObject = finalScoreBox;
		homeButton.positionFromBottomLeft(0.01f, 0.01f);
	}
	
	void removeStatsPanel()
	{
		finalScoreBox.destroy();
		finalScoreText.destroy();
		totalESCatchedText.destroy();
		totalESMissedText.destroy();
		totalESSpawnedText.destroy();
		totalGameTimeText.destroy();
		totalComboText.destroy();
		totalEvolutionsText.destroy();
		leaderboardButton.destroy();
		homeButton.destroy();
	}
}