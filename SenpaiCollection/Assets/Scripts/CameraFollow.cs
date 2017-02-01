using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public float smoothing = 5.0f;

	private Vector3 m_offset;

	// Use this for initialization
	void Start () {
		m_offset = transform.position - target.position;
	}

	void FixedUpdate() {
		Vector3 targetCamPos = target.position + m_offset;
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
		
}
