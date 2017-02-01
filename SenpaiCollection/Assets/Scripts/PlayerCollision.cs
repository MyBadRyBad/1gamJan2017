using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {
	
	private PlayerController m_playerController;

	void Awake() {
		m_playerController = GetComponent<PlayerController> ();
		if (!m_playerController) {
			Debug.LogError ("PlayerController could not be found.");
		}
	}


	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Enemy")) {
			// remove the enemy
	//		Destroy(other.gameObject);
			EnemyMovement enemyMovement = other.gameObject.GetComponent<EnemyMovement>();
			enemyMovement.AnimateCollision ();

			//gameObject.transform.LookAt(
			gameObject.transform.LookAt(other.gameObject.transform.position);
			m_playerController.Dive();


			// update the points
			GameManager.gm.AddPoints(1);
			GameManager.gm.AddTime (5.0f);
		}
	}
}
