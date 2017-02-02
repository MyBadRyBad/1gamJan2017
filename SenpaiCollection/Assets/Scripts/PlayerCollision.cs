using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	// particle system to instantiate when colliding
	public ParticleSystem explosionPrefab;

	// audio clips to play when colliding
	public AudioClip[] beginDiveClips;
	public AudioClip[] endDiveClips;


	// references
	private PlayerController m_playerController;
	private AudioSource m_audioSource;



	void Awake() {
		m_playerController = GetComponent<PlayerController> ();
		if (!m_playerController) {
			Debug.LogError ("PlayerController could not be found.");
		}

		m_audioSource = GetComponent<AudioSource> ();
	}
		

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Enemy")) {
			EnemyMovement enemyMovement = other.gameObject.GetComponent<EnemyMovement>();

			if (!enemyMovement.DidCollapse () && !m_playerController.IsDiving() && m_playerController.CollisionEnabled()) {
				
				// have the enemy face the player
				other.gameObject.transform.LookAt(transform);

				// animate the enemy
				enemyMovement.AnimateCollisionWithDelay (.7f);

				// disable collision so that player can travel max distance for dive
				Physics.IgnoreCollision(other.gameObject.GetComponent<Collider>(), GetComponent<Collider>());

				// play begin dive audio
				PlayRandomAudioClip(beginDiveClips, false);

				// dive towards the enemy
				transform.LookAt(other.gameObject.transform);
				m_playerController.Dive();

				StartCoroutine (CreateExplosion (gameObject.transform, 1.3f));
				StartCoroutine (DestroyObject (other.gameObject, 1.3f));
				StartCoroutine (UpdateGameManager (1.3f));
			}
				
		}
	}

	IEnumerator UpdateGameManager(float delay) {
		yield return new WaitForSeconds (0.8f);

		GameManager.gm.CreateAddPointText ();
		GameManager.gm.CreateAddTimeText ();

		yield return new WaitForSeconds (delay - 0.8f);

		// update the points
		GameManager.gm.AnimatePointsTextMesh();
		GameManager.gm.AnimateTimeTextMesh ();
		GameManager.gm.AddPoints(1);
		GameManager.gm.AddTime (5.0f);
		GameManager.gm.ShowSenpaiGetText ();
	}

	IEnumerator CreateExplosion(Transform t, float delay) {
		yield return new WaitForSeconds (delay);

		GameManager.gm.ShakeCamera ();
		PlayRandomAudioClip (endDiveClips, false);

		if (explosionPrefab) {
			Instantiate (explosionPrefab, t.position, t.rotation); 
		}
	}

	IEnumerator DestroyObject(GameObject g, float delay) {
		yield return new WaitForSeconds (delay);
		GameObject.Destroy (g);
	}

	void PlayRandomAudioClip(AudioClip[] clips, bool canPlayNothing) {
		if (clips.Length > 0) {
			int min = (canPlayNothing) ? -1 : 0;
			int index = Random.Range (min, clips.Length);

			// play only if possible i.e. !canPlayNothing || canPlayNothing && random.range has valid index
			if (index >= 0) {
				m_audioSource.PlayOneShot (clips [index]);
			}
		}
	}
}
