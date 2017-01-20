using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float maxSpeed;
	private bool m_facingRight;


	private Rigidbody2D m_rigidBody2D;

	// Use this for initialization
	void Start () {

		// get a reference for the rigid body.
		m_rigidBody2D = GetComponent<Rigidbody2D> ();
		if (!m_rigidBody2D) {
			Debug.Log ("Rigidbody2D not found. Adding one...");
			m_rigidBody2D = gameObject.AddComponent<Rigidbody2D> ();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void FixedUpdate() 
	{
		// get player input.
		float x_movement = Input.GetAxisRaw ("Horizontal");
		float y_movement = Input.GetAxisRaw ("Vertical");

		float currentSpeed = maxSpeed;

		// if the player is moving in a diagonal, slow them down
		if (Mathf.Abs (x_movement) > 0 && Mathf.Abs (y_movement) > 0) {
			currentSpeed = currentSpeed * 0.75f;
		}

		// update rigid body so that the player moves according to input
		m_rigidBody2D.velocity = new Vector2 (x_movement * currentSpeed, y_movement * currentSpeed);
	}
}
