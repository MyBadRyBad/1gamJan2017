using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D coll) {
		Debug.Log ("collision detected: Collision");

		if (coll.gameObject.tag == "Robot") {
			Debug.Log ("collided with robot.");
		}
	}
}
