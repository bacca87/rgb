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
	public Color color = CustomColor.Red;
	public MoveType moveType = MoveType.Normal;
	public float moveSpeed = 0;
	public float timeToCenter = 10;
	public float rotationSpeed = 5;
	public float sleepTime = 0;
	public bool stop = false;
	public bool isFast = false;
	public Vector3 destination = Vector3.zero;
	
	// Spiral variables
	public float spiralAngularSpeed = 1;
	
	// SineWave variables
	public float sinusAmplitude = 1;
	public float sinusFrequency = 1;
	public float originalAngle = 0;
	
	// Common variables
	private float angle = 0;
	private float distance = 0;
	private float startTime = 0;
	private TrailRenderer trailRenderer = null;
	private bool isPaused = false;
	
	// Use this for initialization
	void Start() 
	{
		// Init variables
		stop = false;
		
		// Trail renderer init
		renderer.material.color = color;
		trailRenderer = GetComponent<TrailRenderer>();		
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
