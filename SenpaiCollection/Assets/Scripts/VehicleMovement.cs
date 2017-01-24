using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour {

	public Vector3 direction;
	public Vector3 rotation;
	public float speed = 2.0f;

	private Rigidbody m_rigidBody;

	void Awake() {
		m_rigidBody = GetComponent<Rigidbody> ();
	}

	// Use this for initialization
	void Start () {
		transform.rotation = Quaternion.Euler (rotation);
	}
	
	// Update is called once per frame
	void Update () {
		// Normalise the movement vector and make it proportional to the speed per second.
		direction = direction.normalized * speed * Time.deltaTime;

		// Move the player to it's current position plus the movement.
		m_rigidBody.MovePosition (transform.position + direction);
	}
}
