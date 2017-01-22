using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {

	// how long should the enemy wander before leaving
	public float wanderDuration = 10.0f;

	// nav mesh agent for moving around level
	private NavMeshAgent m_navMeshAgent;

	// roaming points to mimic wandering
	private float m_roamPointDuration = 3.0f;
	private float m_roamPointTimer;

	#region Unity callbacks
	void Awake() {
		m_navMeshAgent = GetComponent<NavMeshAgent> ();
	}
		
	// Update is called once per frame
	void Update () {
		RefreshRoamPoint ();
		followPosition (GetRandomRoamPoint (-2.5f, 26.5f, -17.5f, -0.5f));

	}
	#endregion
		
	#region Roaming Methods 
	void followPosition (Vector3 position) {
		m_navMeshAgent.SetDestination (position);
	}

	void RefreshRoamPoint() {
		m_roamPointTimer -= Time.deltaTime;

		if (m_roamPointTimer <= 0.0f) {
			m_roamPointTimer = m_roamPointDuration;
		}
	}


	Vector3 GetRandomRoamPoint(float min_x, float max_x, float min_z, float max_z) {
		float x = Random.Range (min_x, max_x);
		float z = Random.Range (min_z, max_z);
		return new Vector3 (x, 0.0f, z);
	}
	#endregion
}
