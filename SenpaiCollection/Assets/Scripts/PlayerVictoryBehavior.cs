using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVictoryBehavior : MonoBehaviour {

	public AudioClip highscoreClip;
	public AudioClip winClip;
	public AudioClip loseClip;

	private AudioSource m_audioSource;
	private QuerySDMecanimController m_sdMecanimController;
	private QuerySDEmotionalController m_sdEmotionalController;

	void Awake() {
		m_audioSource = GetComponent<AudioSource> ();
		m_sdMecanimController = GetComponent<QuerySDMecanimController> ();
		m_sdEmotionalController = GetComponent<QuerySDEmotionalController> ();
	}

	// Use this for initialization
	void Start () {
	}

	public void Idle() {
		m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_IDLE);
	}

	public void WinHighScore() {
		m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_POSE_HELLO);

		if (!m_audioSource.isPlaying)
			m_audioSource.PlayOneShot (highscoreClip);
	}

	public void Win() {
		m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_POSE_CUTE);

		if (!m_audioSource.isPlaying)
			m_audioSource.PlayOneShot (winClip);
	}

	public void Lose() {
		m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_POSE_SIT);
		m_sdEmotionalController.ChangeEmotion (QuerySDEmotionalController.QueryChanSDEmotionalType.NORMAL_SAD);

		if (!m_audioSource.isPlaying)
			m_audioSource.PlayOneShot (loseClip);
	}
}
