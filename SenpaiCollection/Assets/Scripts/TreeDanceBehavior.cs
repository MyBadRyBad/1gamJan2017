using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDanceBehavior : MonoBehaviour {

	private Animator m_anim;
	private BeatObserver m_beatObserver;

	private bool m_translateUp = false;

	// Use this for initialization
	void Start () {
		m_anim = GetComponent<Animator> ();
		m_beatObserver = GetComponent<BeatObserver> ();
		m_translateUp = (Random.Range (0, 100) < 50) ? true : false;

	}
	
	// Update is called once per frame
	void Update () {
		if ((m_beatObserver.beatMask & SynchronizerData.BeatType.DownBeat) == SynchronizerData.BeatType.DownBeat) {
			m_anim.SetTrigger ("DownBeatTrigger");
		}

		if ((m_beatObserver.beatMask & SynchronizerData.BeatType.OnBeat) == SynchronizerData.BeatType.OnBeat) {
			Vector3 t = (m_translateUp) ? (Vector3.forward * 0.05f) : (Vector3.back * 0.05f);
			transform.Translate (t);
			m_translateUp = !m_translateUp;
		} 
	}
}
