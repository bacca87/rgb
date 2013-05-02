using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

[System.Serializable]
public class ScoreoidManager : MonoBehaviour 
{
	// Game info (Scoreoid data)
	public string apiKey = "89e6060105ba77122c56aa72818c6ec435f6e3e2";
	public string gameID = "1b50bd78ef";
	private string response = "xml"; // "xml" or "json"
	
	// User Profile (Scoreoid data)
	public string username;
	public string password;
	public int bestScore;
	public int timePlayed;
	public int rank;
	
	// Leaderboard data (Scoreoid data)
	public Dictionary<string, int> leaderboard;
	public Dictionary<string, int> friendsLeaderboard;
	
	// Return Messages Targets
	public LegacyGui legacyGui;
	
	void Start()
	{
		downloadRGBMaster(); // bastano i dati hardcoded per questo metodo
		leaderboard = new Dictionary<string, int>();
		friendsLeaderboard = new Dictionary<string, int>();
		
		// IL PLAYERPREFS DELL'APPLICAZIONE VA SETTATO DA QUALCHE PARTE NELLA GUI
		// NELLA SPECIFICA FUNZIONE DI AGGIUNTA DEGLI AMICI.
		// MAGARI SI FA UNA "CERCA AMICO" CHE DATO USERNAME RITORNA SE ESISTE, RANK
		// BEST_SCORE, E TIME PLAYED, TRAMITE getPlayerField() DI SCOREOID, E POI
		// UNA "SALVA AMICO", CHE SALVA LO USERNAME IN PLAYERPREFS
		// QUI CREO UNA ENTRY "Friends" IN PLAYERPREFS TEMPORANEA DI TEST FINCHE 
		// NON CE LA GUI.
		if(!PlayerPrefs.HasKey("Friends"))
			PlayerPrefs.SetString("Friends", "nigger,diocane,dioporco,dioladro");
		// N.B.: BISOGNA RENDERE IMPOSSIBILE L'INSERIMENTO DI NOMI CON LA ","
		// 		 BISOGNA RENDERE IMPOSSIBILE AGGIUNGERE USERNAME NON VALIDI (METODO DETTO SOPRA VA BENE)
	}
	
	// Registration
	public IEnumerator createUser()
	{
        string url = "https://www.scoreoid.com/api/createPlayer";
        WWWForm form = new WWWForm();

		form.AddField( "api_key", apiKey );
        form.AddField( "game_id", gameID);
        form.AddField( "response", response);
        form.AddField( "username", username);
		form.AddField( "password", password);

		WWW www = new WWW(url, form);
        yield return StartCoroutine(WaitForCreateUserResponse(www));
	}
	
	IEnumerator WaitForCreateUserResponse(WWW www)
    {	
        yield return www;
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
			XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(www.text));
			while (reader.Read())
			{
				if (reader.Name == "success")
				{
					reader.Read();
					if(reader.NodeType == XmlNodeType.Text)
					legacyGui.responseMessage = reader.Name;
				}
				else if (reader.Name == "error")
				{
					reader.Read();
					if(reader.NodeType == XmlNodeType.Text)
						legacyGui.responseMessage = reader.Value.ToString();
				}
			}
        } 
		else 
		{
            Debug.Log("WWW Error: "+ www.error);
			XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(www.text));
			if (reader.Name == "error")
			{
				reader.Read();
				if(reader.NodeType == XmlNodeType.Text)
					legacyGui.responseMessage = reader.Value.ToString();
			}
        }  
    }
	
	// Login
	public IEnumerator downloadUserData()
	{
        string url = "https://www.scoreoid.com/api/getPlayer";
        WWWForm form = new WWWForm();

		form.AddField( "api_key", apiKey );
        form.AddField( "game_id", gameID);
        form.AddField( "response", response);
        form.AddField( "username", username);
		form.AddField( "password", password);

		WWW www = new WWW(url, form);
        yield return StartCoroutine(WaitForUserDataRequest(www));
	}
	
	IEnumerator WaitForUserDataRequest(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
			XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(www.text));
			while (reader.Read())
				if (reader.Name == "player")
				{
					print("Found player");
					legacyGui.responseMessage = "Login successfull";
					if (reader.MoveToAttribute("best_score"))
						bestScore = int.Parse (reader.Value);
					if (reader.MoveToAttribute("rank"))
						rank = int.Parse (reader.Value);
					if (reader.MoveToAttribute("time_played"))
						timePlayed = int.Parse (reader.Value);
				}
				else if (reader.Name == "error")
				{
					reader.Read();
					if(reader.NodeType == XmlNodeType.Text)
						legacyGui.responseMessage = reader.Value.ToString();
				}
        } 
		else 
		{
            Debug.Log("WWW Error: "+ www.error);
			XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(www.text));
			if (reader.Name == "error")
			{
				reader.Read();
				if(reader.NodeType == XmlNodeType.Text)
					legacyGui.responseMessage = reader.Value.ToString();
			}
        }    
    }
	
	// Global Leaderboard
	public void downloadBestScores(int limit)
	{
        string url = "https://www.scoreoid.com/api/getBestScores";
        WWWForm form = new WWWForm();

		form.AddField( "api_key", apiKey );
        form.AddField( "game_id", gameID);
        form.AddField( "response", response);
        form.AddField( "order_by", "score"); // "score" or "date"
		form.AddField( "order", "desc"); // "desc" or "asc"
		form.AddField( "limit", limit); // volendo si può mettere anche un range per fare le pagine di leaderboard "10,20"

		WWW www = new WWW(url, form);
        StartCoroutine(WaitForBestScoresRequest(www));
	}
	
	IEnumerator WaitForBestScoresRequest(WWW www)
    {
        yield return www;

		if (www.error == null)
        {
//            Debug.Log("WWW Ok!: " + www.text);
			XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(www.text));
			string currentUsername;
			int currentScore;
			while (reader.Read())
			{
				currentScore = 0;
				currentUsername = "";
				if (reader.Name == "scores")
//					Debug.Log("Found some BestScores");
				if (reader.Name == "player")
				{
					print("Found player best score");
					if (reader.MoveToAttribute("username"))
						currentUsername = reader.Value;
					reader.Read();
					if (reader.Name == "score")
						if (reader.MoveToAttribute("score"))
							currentScore = int.Parse (reader.Value);
					leaderboard.Add(currentUsername, currentScore);
				}
			}
        } 
		else 
		{
            Debug.Log("WWW Error: "+ www.error);
        }    
    }
	
	// Friends leaderboard 
	public void downloadFriendsScores()
	{ 
        string url = "https://www.scoreoid.com/api/getPlayerField";
		string friendsUsernames = PlayerPrefs.GetString("Friends");
		string[] usernames = friendsUsernames.Split(',');
		
		Debug.Log("Found " + usernames.Length + " friends in playerprefs");
		Debug.Log("Found " + usernames[0]);
		
		foreach( string username in usernames)
		{
			Debug.Log("Looking up for " + username);
			WWWForm form = new WWWForm();
		
			form.AddField( "api_key", apiKey );
	        form.AddField( "game_id", gameID);
	        form.AddField( "response", response);
	        form.AddField( "username", username); 
			form.AddField( "field", "best_score");
			
			WWW www = new WWW(url, form);
	        StartCoroutine(WaitForFriendBestScoreRequest(www, username));
		}
	}
	
	IEnumerator WaitForFriendBestScoreRequest(WWW www, string username)
    {
        yield return www;

		if (www.error == null)
        {
//            Debug.Log("WWW Ok!: " + www.text);
			XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(www.text));
				if (reader.Name == "best_score")
					friendsLeaderboard.Add( username, int.Parse(reader.Value) );
        } 
		else 
		{
//            Debug.Log("WWW Error: "+ www.error);
        }    
    }	
	
	// RGB Master get
	public void downloadRGBMaster()
	{
        string url = "https://www.scoreoid.com/api/getBestScores";
        WWWForm form = new WWWForm();

		form.AddField( "api_key", apiKey );
        form.AddField( "game_id", gameID);
        form.AddField( "response", response);
		form.AddField( "order_by", "score");
		form.AddField( "order", "desc");
		form.AddField( "limit", 1); 

		WWW www = new WWW(url, form);
        StartCoroutine(WaitForRGBMasterRequest(www));
	}
	
	IEnumerator WaitForRGBMasterRequest(WWW www)
    {
        yield return www;
		if (www.error == null)
        {
			XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(www.text));
			while (reader.Read())
			{
				if (reader.Name == "player")
				{
					if (reader.MoveToAttribute("username"))
						PlayerPrefs.SetString("RGBMasterUsername", reader.Value);
					reader.Read();
					if (reader.Name == "score")
						if (reader.MoveToAttribute("score"))
							PlayerPrefs.SetInt("RGBMasterScore", int.Parse(reader.Value));
				}
			}
			print ("RGBMaster is: " + PlayerPrefs.GetString("RGBMasterUsername"));
        }    
    }
	
	// Score upload
	public void uploadScore(int score)
	{
        string url = "https://www.scoreoid.com/api/createScore";
        WWWForm form = new WWWForm();

		form.AddField( "api_key", apiKey );
        form.AddField( "game_id", gameID);
        form.AddField( "response", response);
        form.AddField( "username", username);
//		form.AddField( "password", password);
		form.AddField( "username", score);

		WWW www = new WWW(url, form);
        StartCoroutine(WaitForCreateUserResponse(www));
	}
	
	IEnumerator WaitForUploadScoreResponse(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
			XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(www.text));
				if (reader.Name == "success")
					Debug.Log(reader.Value);
        } 
		else 
		{
			Debug.Log("WWW Error: "+ www.error);
			XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(www.text));
				if (reader.Name == "error")
					Debug.Log(reader.Value);
        }    
    }
}

//DATI PLAYER SUPPORTATI DA SCOREOID
//username => The players username [String]
//password => The players password [String] 
//unique_id => The players unique ID [Integer] 
//first_name => The players first name [String]  
//last_name => The players last name [String] 
//email => The players email [String] 
//created => The date the player was created calculated by Scoreoid [YYYY-MM-DD hh:mm:ss]
//updated => The last time the player was updated calculated by Scoreoid [YYYY-MM-DD hh:mm:ss]
//bonus => The players bonus [Integer] 
//achievements => The players achievements [String, comma-separated] 
//best_score => The players best score calculated by Scoreoid [Integer]
//gold => The players gold [Integer] 
//money => The players money [Integer] 
//kills => The players kills [Integer] 
//lives => The players lives [Integer] 
//time_played => The time the player played [Integer] 
//unlocked_levels => The players unlocked levels [Integer] 
//unlocked_items => The players unlocked items [String, comma-separated] 
//inventory => The players inventory [String, comma-separated] 
//last_level => The players last level [Integer] 
//current_level => The players current level [Integer] 
//current_time => The players current time [Integer] 
//current_bonus => The players current bonus [Integer] 
//current_kills => The players current kills [Integer] 
//current_achievements => The players current achievements [String, comma-separated] 
//current_gold => The players current gold [Integer] 
//current_unlocked_levels => The players current unlocked levels [Integer] 
//current_unlocked_items => The players current unlocked items [String, comma-separated] 
//current_lifes => The players current lifes [Integer] 
//xp => The players XP [Integer] 
//energy => The players energy [Integer] 
//boost => The players energy [Integer] 
//latitude => The players GPS latitude [Integer] 
//longitude => The players GPS longitude [Integer] 
//game_state => The players game state [String]
//platform => The players platform needs to match the string value that was used when creating the player [String]
//rank => The players current rank [Integer] 
//
//DATI GAME PER CONNESSIONE A SCOREID
//api_key=> Your API Key [Required]
//game_id => Your Game ID [Required]
//response => String Value, "XML" or "JSON" [Required]