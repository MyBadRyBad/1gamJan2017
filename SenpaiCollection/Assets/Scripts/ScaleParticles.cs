using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleParticles : MonoBehaviour {

	private ParticleSystem m_particleSystem;

	// Use this for initialization
	void Start () {
		m_particleSystem = GetComponent<ParticleSystem> ();
		ParticleScaler.Scale (m_particleSystem, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
