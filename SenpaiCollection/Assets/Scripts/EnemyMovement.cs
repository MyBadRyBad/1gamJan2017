﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {

	// how long should the enemy wander before leaving
	public float wanderDuration = 20.0f;

	// reference to the animation
	private Animation m_animation;

	// array of points where enemy will exit to when wandering is done
	private GameObject[] m_exitPoints;

	// flag to prevent wanderDuration from running if enemy spawned outside of roam space
	private bool m_canRoam = false;

	// flags to trigger exit 
	[SerializeField]
	private bool m_shouldExit = false;
	[SerializeField]
	private bool m_isExiting = false;

	// nav mesh agent for moving around level
	private NavMeshAgent m_navMeshAgent;

	// roaming points to mimic wandering
	[SerializeField]
	private float m_roamPointDuration = 6.0f;
	private float m_roamPointTimer;

	// wander timer
	[SerializeField]
	private float m_wanderTimer;

	// min and max coordinates for space to wander
	private float m_roampointMinX = -2.5f;
	private float m_roampointMaxX = 26.5f;
	private float m_roampointMinZ = -17.5f;
	private float m_roampointMaxZ = -0.5f;

	#region Unity callbacks
	void Awake() {
		m_navMeshAgent = GetComponent<NavMeshAgent> ();

		// setup wanderTimer
		m_wanderTimer = wanderDuration;

		// setup roam timer
		m_roamPointTimer = 0.0f;

		// get reference to animator
		m_animation = GetComponent<Animation>();
	}

	void Start() {
		m_exitPoints = GameObject.FindGameObjectsWithTag ("ExitPoint");

		if (m_exitPoints.Length == 0) {
			Debug.LogError("ExitPoints for " + gameObject.name + "is empty.");
		}
	}
		
	// Update is called once per frame
	void Update () {
		UpdateAnimation ();

		if (!m_canRoam) 
			TriggerWanderDuration ();

		if (m_shouldExit) {
			GoToRandomExit ();
		} 

		if (!m_isExiting) {
			RefreshRoamPoint ();
			UpdateWanderTimer ();
		}
	}
	#endregion


	#region Trigger Wander methods
	void TriggerWanderDuration () {
		bool withinX = (transform.position.x >= m_roampointMinX) && (transform.position.x <= m_roampointMaxX);
		bool withinZ = (transform.position.z >= m_roampointMinZ) && (transform.position.z <= m_roampointMaxZ);

		m_canRoam = (withinX && withinZ);
	}
	#endregion

	#region Go To Exit methods
	void GoToRandomExit() {
		if (!m_isExiting) {
			// Set destination to a random exit point
			int exitPointIndex = Random.Range (0, m_exitPoints.Length);
			m_navMeshAgent.SetDestination (m_exitPoints [exitPointIndex].transform.position);

			// flag isExiting to true to prevent a different exit point from being selected
			m_isExiting = true;
		}
	}

	void UpdateWanderTimer() {
		m_wanderTimer -= Time.deltaTime;

		if (m_wanderTimer <= 0.0f) {
			m_shouldExit = true;
		}
	}

	#endregion


	#region Animation Methods
	void UpdateAnimation() {
		if (m_navMeshAgent.velocity.x > 0 || m_navMeshAgent.velocity.z > 0 || m_navMeshAgent.velocity.y > 0) {
		} else {
			m_animation.Play ("idle_01");
		}

	}


	#endregion

	#region Roaming Methods 
	void FollowPosition (Vector3 position) {
		m_navMeshAgent.SetDestination (position);
	}

	void RefreshRoamPoint() {
		// decrement timer 
		m_roamPointTimer -= Time.deltaTime;

		// timer has reached zero, so update following to anew position
		if (m_roamPointTimer <= 0.0f) {
			FollowPosition (GetRandomRoamPoint (m_roampointMinX, m_roampointMaxX, m_roampointMinZ, m_roampointMaxZ));
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
