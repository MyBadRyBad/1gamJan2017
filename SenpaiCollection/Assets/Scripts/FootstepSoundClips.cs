using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSoundClips : MonoBehaviour {

	// audio clips to play for footsteps
	public AudioClip[] roadClips;
	public AudioClip[] grassClips;


	// reference to the audio Source and controller
	private AudioSource m_audioSource;
	private PlayerController m_playerController;

	// length between each step
	[SerializeField] private float m_audioLengthStepRun = 0.1f;
	[SerializeField] private float m_audioLengthStepWalk = 0.2f;

	// flag to trigger step sound
	private bool m_canStep = true;

	void Awake() {
		m_playerController = GetComponent<PlayerController> ();

		// create a new Audio source strictly for footsteps
		m_audioSource = gameObject.AddComponent<AudioSource> ();

	}

	void OnCollisionStay(Collision coll) {
		if (coll.gameObject.CompareTag ("Road") || coll.gameObject.CompareTag ("SideWalkBorder")) {
			if (m_playerController.IsDashing() && m_canStep) {
				PlayFootSteps (roadClips, m_audioLengthStepRun);
			} else if (m_playerController.IsWalking() && m_canStep) {
				PlayFootSteps (roadClips, m_audioLengthStepWalk);
			}
		} else if (coll.gameObject.CompareTag ("Grass")) {
			if (m_playerController.IsDashing() && m_canStep) {
				PlayFootSteps (grassClips, m_audioLengthStepRun);
			} else if (m_playerController.IsWalking() && m_canStep) {
				PlayFootSteps (grassClips, m_audioLengthStepWalk);
			}
		}
	}

	void PlayFootSteps(AudioClip[] clips, float delay) {
		m_audioSource.clip = clips[Random.Range(0, clips.Length)];
		m_audioSource.volume = 0.1f;
		m_audioSource.Play();
		StartCoroutine(WaitForFootSteps(delay));
	}
		
	IEnumerator WaitForFootSteps(float delay) {
		m_canStep = false; 
		yield return new WaitForSeconds(delay); 
		m_canStep = true; 
	}
}
