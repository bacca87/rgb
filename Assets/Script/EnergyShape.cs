using UnityEngine;
using System.Collections;

public enum MoveType
{
	Normal,
	Spyral,
	SineWawe
}

[RequireComponent(typeof(Rigidbody))]
public class EnergyShape : MonoBehaviour 
{
	public Color color { get; set; }
	public MoveType moveType { get; set; }
	public float moveSpeed { get; set; }
	public float timeToCenter { get; set; }
	public float rotationSpeed { get; set; }
	public float sleepTime { get; set; }
	public bool stop { get; set; }
	public bool isFast { get; set; }
	public Vector3 destination { get; set; }
	
	// Spiral variables
	public float spiralAngularSpeed { get; set; }
	
	// SineWave variables
	public float sinusAmplitude { get; set; }
	public float sinusFrequency { get; set; }
	public float originalAngle { get; set; }
	
	// Common variables
	private float angle { get; set; }
	private float distance { get; set; }
	private float startTime { get; set; }
	private TrailRenderer trailRenderer { get; set; }
	private bool isPaused { get; set; }
	
	// Use this for initialization
	void Awake() 
	{
		// Trail renderer init
		trailRenderer = GetComponent<TrailRenderer>();
		Initialize();
	}
	
	//Inizializza tutti i valori di default
	public void Initialize()
	{
		color = CustomColor.Red;
		moveType = MoveType.Normal;
		moveSpeed = 1;
		timeToCenter = 10;
		rotationSpeed = 5;
		sleepTime = 0;
		stop = false;
		isFast = false;
		destination = Vector3.zero;
		
		// Spiral variables
		spiralAngularSpeed = 40;
		
		// SineWave variables
		sinusAmplitude = 15;
		sinusFrequency = 3;
		originalAngle = 0;
		
		// Common variables
		angle = 0;
		distance = 0;
		startTime = 0;
		isPaused = false;
	}
	
	//prepara l'ES a partire
	public void Prepare()
	{
		// Init variables
		stop = false;
		renderer.material.color = color;
		
		// Trail renderer init
		if(!isFast) trailRenderer.enabled = false;
		
		// Recalculate distance from center 
		distance = (transform.position - destination).magnitude;
		// Recalculate angle of spawn
		if( transform.position.y >= 0 )
			angle = Mathf.Acos(transform.position.x/distance) * Mathf.Rad2Deg;
		else
			angle = 360 - Mathf.Acos(transform.position.x/distance) * Mathf.Rad2Deg;
		
		originalAngle = angle;
		
		moveSpeed = distance / timeToCenter;
		startTime = Time.time;
	}
	
	void Pause()
	{
		isPaused = true;
		trailRenderer.time = 9999; // workaround per non eliminare la scia quando metti in pausa
		rigidbody.isKinematic = true;		
	}
	
	void Resume()
	{
		isPaused = false;
		rigidbody.isKinematic = false;
	}
	
	// Update is called once per frame
	void FixedUpdate() 
	{
		if(stop || isPaused) return;
		if(Time.time - startTime < sleepTime) return;
		
		rigidbody.AddTorque(0, 0, rotationSpeed);
		
		switch(moveType)
		{
		case MoveType.Normal:
			rigidbody.MovePosition(transform.position + (destination - transform.position).normalized * Time.deltaTime * moveSpeed);
			break;
		case MoveType.Spyral:
			distance = distance - moveSpeed * Time.deltaTime;
			angle = (angle + spiralAngularSpeed * Time.deltaTime);
			rigidbody.MovePosition(new Vector3(distance * Mathf.Cos(angle * Mathf.Deg2Rad), distance * Mathf.Sin(angle * Mathf.Deg2Rad),0) + destination);
			break;
		case MoveType.SineWawe:	
			distance = distance - moveSpeed * Time.deltaTime;
			angle = originalAngle * Mathf.Deg2Rad + Mathf.Sin(Time.time * sinusFrequency) * sinusAmplitude * Mathf.Deg2Rad;
			rigidbody.MovePosition(new Vector3(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle),0) + destination);
			break;
		default:
			break;
		}
	}
}
