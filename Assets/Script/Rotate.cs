using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour 
{
	public float speed = 5;
	
	// Update is called once per frame
	void Update () 
	{
		//ruota semplicemente l'oggetto sull'asse z
		transform.Rotate(0,0,speed * Time.deltaTime);	
	}
}
