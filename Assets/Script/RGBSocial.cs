using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RGBSocial : MonoBehaviour 
{	
	static int esCatched = 0;
	static float totalGameTime = 0;
	static int completedGames = 0;
	
	// flag per far eseguire una sola volta l'achievements
	static bool min5 = true;
	static bool min6 = true;
	static bool min7 = true;	
	static bool hour1 = true;
	static bool hour5 = true;
	static bool hour10 = true;
	
	static bool loggedIn = false;
	
	static bool runOnce = true;
	
	void OnGUI()
	{
		GUIStyle style1 = new GUIStyle();
		style1.fontSize = 18;
		style1.normal.textColor = Color.white;
		
		GUIContent time = new GUIContent();		
		time.text = "Time: " + totalGameTime;
		
		GUIContent catched = new GUIContent();
		catched.text = "esCatched: " + esCatched;
		
		GUIContent completed = new GUIContent();
		completed.text = "Completed: " + completedGames;
		
		GUI.Label(new Rect(0,40,100,50), time, style1);
		GUI.Label(new Rect(0,60,100,50), catched, style1);
		GUI.Label(new Rect(0,80,100,50), completed, style1);
	}
	
	void Start()
	{
		if(runOnce)
		{
			SwarmLoginManager.addLoginListener(delegate(int status) 
			{
				if (status == SwarmLoginManager.USER_LOGGED_IN) 
				{
					loggedIn = true;
				
					load();
					checkPaidVersion();
				} 
				else if (status == SwarmLoginManager.LOGIN_STARTED) 
				{
				// The player has started logging in
				} 
				else if (status == SwarmLoginManager.LOGIN_CANCELED) 
				{
				// The player has cancelled the login
				} 
				else if (status == SwarmLoginManager.USER_LOGGED_OUT) 
				{
					loggedIn = false;
				}
			});
	
			Swarm.init(SwarmConsts.App.APP_ID, SwarmConsts.App.APP_AUTH);
		}
	}
		
	void load()
	{
		//ho usato il += perche se è lento a caricare somma ai dati calcolati in locale
		print("[begin loading cloud data]");
		
		SwarmActiveUser.getUserData("ESCatched", delegate(string responseData) 
		{
			if (responseData != null) 
			{
				esCatched += int.Parse(responseData);
				print("[esCatched = " + esCatched + "]");
			}
			else
				print("[ESCatched = NULL]");
		});
		
		SwarmActiveUser.getUserData("TotalGameTime", delegate(string responseData) 
		{
			if (responseData != null) 
			{
				totalGameTime += float.Parse(responseData);
				print("[TotalGameTime = " + totalGameTime + "]");
			}
			else
				print("[TotalGameTime = NULL]");
		});
		
		SwarmActiveUser.getUserData("CompletedGames", delegate(string responseData) 
		{
			if (responseData != null) 
			{
				completedGames += int.Parse(responseData);
				print("[completedGames = " + completedGames + "]");
			}
			else
				print("[CompletedGames = NULL]");
		});
		
		print("[end loading cloud data]");
	}
		
	public void save()
	{
		if(loggedIn)
		{
			SwarmActiveUser.saveUserData("ESCatched", esCatched.ToString());
			SwarmActiveUser.saveUserData("TotalGameTime", totalGameTime.ToString());
			SwarmActiveUser.saveUserData("CompletedGames", completedGames.ToString());
		}
	}
	
	public void submitScore(int score)
	{
		if(loggedIn)
		{
			SwarmLeaderboard.submitScoreGetRank(SwarmConsts.Leaderboard.LEADERBOARD_ID, score, delegate(int rank){checkRGBMaster(rank);});
		}
		else
		{
			SwarmLeaderboard.submitScore(SwarmConsts.Leaderboard.LEADERBOARD_ID, score);
		}
	}
	
	private void unlockAchievement(int id)
	{
		print("[unlock achievement = " + id + "]");
		SwarmAchievement.unlockAchievement(id);
	}
		
	public void checkRGBMaster(int rank)
	{
		if(rank == 1)
			unlockAchievement(SwarmConsts.Achievement.RGB_MASTER_ID);
	}
	
	public void checkEvolution(Shape shape, bool hasErrors)
	{
		switch(shape)
		{
		case Shape.Square:
			unlockAchievement(SwarmConsts.Achievement.SQUARE_ID);
			if(!hasErrors) unlockAchievement(SwarmConsts.Achievement.SUPER_SQUARE_ID);
			break;
		case Shape.Pentagon:
			unlockAchievement(SwarmConsts.Achievement.PENTAGON_ID);
			if(!hasErrors) unlockAchievement(SwarmConsts.Achievement.PERFECT_PENTAGON_ID);
			break;
		case Shape.Hexagon:
			unlockAchievement(SwarmConsts.Achievement.HEXAGON_ID);
			if(!hasErrors) unlockAchievement(SwarmConsts.Achievement.HAPPY_HEXAGON_ID);
			break;
		case Shape.Heptagon:
			unlockAchievement(SwarmConsts.Achievement.HEPTAGON_ID);
			if(!hasErrors) unlockAchievement(SwarmConsts.Achievement.HOT_HEPTAGON_ID);
			break;
		case Shape.Octagon:
			unlockAchievement(SwarmConsts.Achievement.OCTAGON_ID);
			if(!hasErrors) unlockAchievement(SwarmConsts.Achievement.OVERPOWER_OCTAGON_ID);
			break;
		}
	}
	
	public void checkCombo(int comboMultiplier)
	{
		switch(comboMultiplier)
		{
		case 2: 
			unlockAchievement(SwarmConsts.Achievement.COMBO_2X_ID);
			break;
		case 3:
			unlockAchievement(SwarmConsts.Achievement.COMBO_3X_ID);
			break;
		case 4:
			unlockAchievement(SwarmConsts.Achievement.COMBO_4X_ID);
			break;
		case 5:
			unlockAchievement(SwarmConsts.Achievement.COMBO_5X_ID);
			break;
		}	
	}
	
	public void checkPaidVersion()
	{
		//TODO: gestire la differenziazione della versione a pagamento e non
		unlockAchievement(SwarmConsts.Achievement.SHUT_UP_AND_TAKE_MY_MONEY_ID);
	}
		
	public void checkSessionTime(float time)
	{
		int minutes = (int)time/60;
		
		switch(minutes)
		{
		case 5:
			if(min5)
			{
				min5 = false;
				unlockAchievement(SwarmConsts.Achievement.OUT_IN_5_MINUTES_ID);
			}
			break;
		case 6:
			if(min6)
			{
				min6 = false;
				unlockAchievement(SwarmConsts.Achievement._6_MINUTES_ID);
			}
			break;
		case 7:
			if(min7)
			{
				min7 = false;
				unlockAchievement(SwarmConsts.Achievement._7_MINS_ID);
			}
			break;
		}
	}
	
	public void checkCatchedES()
	{
		esCatched++;
		
		if(esCatched > 100000) //stop incremento
			esCatched = 100001; //ATTENZIONE = per far smettere di updatare l'achievement
		
		switch(esCatched)
		{
		case 1000:
			unlockAchievement(SwarmConsts.Achievement.ITS_OVER_1_THOUSAAAAND_ID);
			break;
		case 10000:
			unlockAchievement(SwarmConsts.Achievement._10000_ID);
			break;
		case 100000:
			unlockAchievement(SwarmConsts.Achievement.SERIOUSLY_YOU_PLAY_TOO_MUCH_ID);
			break;
		}
	}
	
	public void checkTotalPlayTime(float deltaTime)
	{
		//ATTENZIONE: chiamata ad ogni frame
		totalGameTime += deltaTime;
		
		if(totalGameTime > (3600*10)) //stop incremento
			totalGameTime = (3600*11); //ATTENZIONE = 11 ore per far smettere di updatare l'achievement
		
		int hours = (int)totalGameTime/3600;
				
		switch(hours)
		{
		case 1:
			if(hour1)
			{
				hour1 = false;
				unlockAchievement(SwarmConsts.Achievement.BEGINNER_ID);
			}
			break;
		case 5:
			if(hour5)
			{
				hour5 = false;
				unlockAchievement(SwarmConsts.Achievement.EXPERT_ID);
			}
			break;
		case 10:
			if(hour10)
			{
				hour10 = false;
				unlockAchievement(SwarmConsts.Achievement.VETERAN_ID);
			}
			break;
		}
	}
	
	public void checkCompletedGames()
	{
		completedGames++;
		
		if(completedGames > 100) //stop incremento
			completedGames = 101; //ATTENZIONE = per far smettere di updatare l'achievement
		
		switch(completedGames)
		{
		case 1:
			unlockAchievement(SwarmConsts.Achievement.THE_FIRST_TIME_ID);
			break;
		case 20:
			unlockAchievement(SwarmConsts.Achievement._20_GAMES_ID);
			break;
		case 50:
			unlockAchievement(SwarmConsts.Achievement._50_GAME_ID);
			break;
		case 100:
			unlockAchievement(SwarmConsts.Achievement._100_GAMES_ID);
			break;
		}
	}
	
	public void checkZeroPointDeath(int points)
	{
		if(points == 0)
			unlockAchievement(SwarmConsts.Achievement.YOURE_DOING_IT_WRONG_ID);
	}
	
	public void checkSprayCount(int sprayCount)
	{
		if(sprayCount > 100)
			unlockAchievement(SwarmConsts.Achievement.GIVE_ME_MOARRRR_ID);
	}
	
	public void checkRotation()
	{
		unlockAchievement(SwarmConsts.Achievement.THE_WORLD_IS_UPSIDE_DOWN_ID);
	}
	
	public void checkInfoRead()
	{
		unlockAchievement(SwarmConsts.Achievement.DEVELOPERSDEVELOPERSDEVELOPERSDEVELOPERS_ID);
	}
}
