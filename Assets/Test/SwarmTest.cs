using UnityEngine;
using System.Collections;

public class SwarmTest : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
//		SwarmLoginManager.addLoginListener(delegate(int status) {
//		if (status == SwarmLoginManager.USER_LOGGED_IN) {
//		// The player has successfully logged in
//		// For example, you may wish to call Application.LoadLevel("MyLevel");
//		} else if (status == SwarmLoginManager.LOGIN_STARTED) {
//		// The player has started logging in
//		} else if (status == SwarmLoginManager.LOGIN_CANCELED) {
//		// The player has cancelled the login
//		} else if (status == SwarmLoginManager.USER_LOGGED_OUT) {
//		// The player has logged out
//		}
//		});
		//Swarm.showLeaderboards();
		SwarmLeaderboard.showLeaderboard(7801);
	}
	
	// Update is called once per frame
	void Update () 
	{	
	}
}
