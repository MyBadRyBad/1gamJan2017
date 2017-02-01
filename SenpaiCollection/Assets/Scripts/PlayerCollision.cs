using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	public ParticleSystem explosionPrefab;

	private PlayerController m_playerController;

	void Awake() {
		m_playerController = GetComponent<PlayerController> ();
		if (!m_playerController) {
			Debug.LogError ("PlayerController could not be found.");
		}
	}


	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Enemy")) {
			EnemyMovement enemyMovement = other.gameObject.GetComponent<EnemyMovement>();

			if (!enemyMovement.DidCollapse ()) {
				
				// have the enemy face the player
				other.gameObject.transform.LookAt(transform);


				// animate the enemy
				enemyMovement.AnimateCollisionWithDelay (.7f);

				// disable collision so that player can travel max distance for dive
				Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());

				// dive towards the enemy
				transform.LookAt(other.gameObject.transform);
				m_playerController.Dive();

				StartCoroutine (createExplosion ());

				// update the points
				GameManager.gm.AddPoints(1);
				GameManager.gm.AddTime (5.0f);
			}
				
		}
	}

	IEnumerator createExplosion() {
		yield return new WaitForSeconds (0.7f);

		if (explosionPrefab) {
			Instantiate (explosionPrefab, transform.position, transform.rotation); 
		}
	}
}
