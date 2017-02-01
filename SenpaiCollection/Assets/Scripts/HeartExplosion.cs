using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartExplosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Explode ();
	}

	void Explode() {
		var exp = GetComponent<ParticleSystem>();
		exp.Play();
		Destroy(gameObject, exp.duration);
	}

}
