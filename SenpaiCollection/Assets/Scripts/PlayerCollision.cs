using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Enemy")) {
			// remove the enemy
	//		Destroy(other.gameObject);
			EnemyMovement enemyMovement = other.gameObject.GetComponent<EnemyMovement>();
			enemyMovement.AnimateCollision ();


			// update the points
			GameManager.gm.AddPoints(1);
			GameManager.gm.AddTime (5.0f);
		}
	}
}
