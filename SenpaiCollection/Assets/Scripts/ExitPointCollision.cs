using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExitPointCollision : MonoBehaviour {

	public Transform despawnPosition;

	void Start() {
		if (!despawnPosition) {
			Debug.LogError ("No despawn set for " + gameObject.name + ".");
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Enemy")) {
			NavMeshAgent navMeshAgent = other.GetComponent<NavMeshAgent> ();
			navMeshAgent.SetDestination (despawnPosition.position);
		}
	}
}
