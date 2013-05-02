using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour 
{
	public float waitTime = 1;
	
	// Use this for initialization
	void Start () 
	{
	    StartCoroutine(loadLevel());
	}
	
	IEnumerator loadLevel()
	{
		yield return new WaitForSeconds(waitTime);
		Application.LoadLevelAdditive("GameScene");
		Destroy(gameObject);
	}
}
