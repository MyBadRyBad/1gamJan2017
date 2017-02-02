using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {

	public enum EnemyState {
		Walking,
		Idle,
		Collapse
	};

	// how long should the enemy wander before leaving
	public float wanderDuration = 20.0f;

	public AudioClip[] audioClips;

	// refernce to the audio source
	private AudioSource m_audioSource;

	// reference to the animation
	private Animation m_animation;

	// array of points where enemy will exit to when wandering is done
	private GameObject[] m_exitPoints;

	// previous position to detect if the object move
	private Vector3 m_previousPosition;

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

	private EnemyState m_state = EnemyState.Idle;

	#region Unity callbacks
	void Awake() {
		m_navMeshAgent = GetComponent<NavMeshAgent> ();

		// setup wanderTimer
		m_wanderTimer = wanderDuration;

		// setup roam timer
		m_roamPointTimer = 0.0f;

		// get references
		m_animation = GetComponent<Animation>();
		m_audioSource = GetComponent<AudioSource> ();

		if (!m_audioSource) {
			m_audioSource = gameObject.AddComponent<AudioSource> ();
		}
	
	}

	void Start() {
		m_exitPoints = GameObject.FindGameObjectsWithTag ("ExitPoint");

		if (m_exitPoints.Length == 0) {
			Debug.LogError("ExitPoints for " + gameObject.name + "is empty.");
		}

		// setup animation
		m_animation ["down_20"].wrapMode = WrapMode.Once;
		m_animation ["down_20"].speed = 0.7f;

		// setup audiosource
		m_audioSource.pitch = 1.2f;
		m_audioSource.volume = 0.2f;
	}
		
	// Update is called once per frame
	void Update () {
		UpdateAnimation ();

		if (!m_canRoam && m_state != EnemyState.Collapse) 
			TriggerWanderDuration ();

		if (m_shouldExit && m_state != EnemyState.Collapse) {
			GoToRandomExit ();
		} 

		if (!m_isExiting && m_state != EnemyState.Collapse) {
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

	#region Audio playback
	void PlayAudio() {
		if (audioClips.Length > 0) {
			int index = Random.Range (0, audioClips.Length);
			m_audioSource.PlayOneShot (audioClips [index]);
		}
	}
	#endregion

	#region Animation Methods
	void UpdateAnimation() {
		if (m_state != EnemyState.Collapse) {
			if (transform.position != m_previousPosition && m_navMeshAgent.velocity != Vector3.zero) {
				m_animation.Play ("walk_00");
				m_state = EnemyState.Walking;
			} else {
				m_animation.Play ("idle_01");
				m_state = EnemyState.Idle;
			}

			m_previousPosition = transform.position;
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

	#region collision methods
	public void AnimateCollisionWithDelay(float delay) {
		StartCoroutine (AnimateCollision (delay));
	}

	IEnumerator AnimateCollision(float delay) {
		// first set the state
		m_state = EnemyState.Collapse;

		// stop the navmesh
		m_navMeshAgent.Stop ();

		// set the animation to idle so the enemy doesn't moonwalk
		m_animation.Play ("idle_01");

		// wait for the delay
		yield return new WaitForSeconds(delay);

		// play audio clips
		PlayAudio();

		// call animation
		m_animation.Play ("down_20");
	}
	#endregion

	#region getter methods
	public bool DidCollapse() {
		return m_state == EnemyState.Collapse;
	}

	public bool IsWalking() {
		return m_state == EnemyState.Walking;
	}

	public bool IsIdle() {
		return m_state == EnemyState.Idle;
	}

	#endregion
}
