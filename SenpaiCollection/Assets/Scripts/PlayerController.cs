using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float defaultSpeed = 2.0f;
	public float dashSpeed = 4.0f;

	private Vector3 m_movement;
	private Animator m_anim;
	private Rigidbody m_playerRigidbody;
	private QuerySDMecanimController m_sdMecanimController;

	private float m_dashDelay = 1.0f;
	private float m_dashDelayTimer;

	private bool m_isDashing = false;

	#region Unity callbacks
	void Awake() {
		// Set up references.
		m_anim = GetComponent <Animator> ();
		m_playerRigidbody = GetComponent <Rigidbody> ();
		m_sdMecanimController = GetComponent<QuerySDMecanimController> ();

		// set up timers
		m_dashDelayTimer = m_dashDelay;
	}
		
	void Start () {
	}

	void Update () {
		
	}

	void FixedUpdate ()
	{
		// Store the input axes.
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");

		// Move the player around the scene.
		Move (horizontal, vertical);

		// Execute dash if player holds down button
		Dash (Input.GetButton ("Jump"));

		// Animate the player.
		Animating (horizontal, vertical);
	}
	#endregion

	#region movement methods
	void Move (float horizontal, float vertical)
	{
		// change speed according to walk/dash state
		float speed = (m_isDashing) ? dashSpeed : defaultSpeed;

		// Set the movement vector based on the axis input.
		m_movement.Set (horizontal, 0f, vertical);

		// Normalise the movement vector and make it proportional to the speed per second.
		m_movement = m_movement.normalized * speed * Time.deltaTime;

		// Move the player to it's current position plus the movement.
		m_playerRigidbody.MovePosition (transform.position + m_movement);

		// rotate the player towards movement direction
		if (horizontal != 0 || vertical != 0) {
			transform.rotation = Quaternion.LookRotation (m_movement);
		}
	}

	void Dash(bool shouldDash) {
		m_isDashing = shouldDash;
	}
	#endregion

	#region animation methods
	void Animating (float h, float v)
	{
		// Create a boolean that is true if either of the input axes is non-zero.
		bool walking = (h != 0f) || (v != 0f);

		if (walking) {
			if (m_isDashing) {
				m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN);
			} else {
				m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_WALK);
			}
		} else {
			m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_STAND);
		}
	}
	#endregion
}
