using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenpaiVictoryPose : MonoBehaviour {

	public string[] animations;

	private Animation m_animation;

	// Use this for initialization
	void Start () {
		m_animation = GetComponent<Animation> ();

		ShowPose ();
	}
		
	void ShowPose() {
		if (m_animation) {
			int index = Random.Range (0, animations.Length);
			m_animation.Play (animations[index]);
		}
	}
}
