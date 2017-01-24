using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// movement speed variables
	public float defaultSpeed = 2.0f;
	public float dashSpeed = 4.0f;

	// dash variables
	public float dashDecreaseRate = 2.75f;
	public float dashIncreaseRate = 1.25f;
	public float dashRefreshDelay = 2.0f;

	// movement and animation
	private Vector3 m_movement;
	private Rigidbody m_playerRigidbody;
	private QuerySDMecanimController m_sdMecanimController;

	// dash delay
	private float m_dashDelayTimer;

	// flag to know when character is dashing
	private bool m_isDashing = false;
	private bool m_dashDelayTriggered = false;


	#region Unity callbacks
	void Awake() {
		// Set up references.
		m_playerRigidbody = GetComponent <Rigidbody> ();
		m_sdMecanimController = GetComponent<QuerySDMecanimController> ();

	}
		
	void Start () {
		// set up timers
		m_dashDelayTimer = dashRefreshDelay;
	}

	void Update () {
		UpdateDash ();
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
		float speed = (m_isDashing && !GameManager.gm.DashBarManager().IsEmpty ()) ? dashSpeed : defaultSpeed;

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
	#endregion

	#region Dashing
	void Dash(bool shouldDash) {
		m_isDashing = shouldDash;
	}

	void UpdateDash() {
		if (m_isDashing && !GameManager.gm.DashBarManager().IsEmpty ()) {
			// update the dash text to animate
			GameManager.gm.UpdateDashTextMesh ("<j>Dash");

			// refresh delay timer 
			SetupDashDelayTimer ();

			// decrease the dash bar 
			GameManager.gm.DashBarManager().Decrease (dashDecreaseRate);

		} 
		else if (!GameManager.gm.DashBarManager().IsFull()) {
			// update the dash text to stop animation
			GameManager.gm.UpdateDashTextMesh ("Dash");

			//  increase the dash bar only if the delay is over.
			if (m_dashDelayTriggered) {
				UpdateDashDelayTimer ();
			} else {
				GameManager.gm.DashBarManager().Increase (dashIncreaseRate);
			}
		}


	}

	void SetupDashDelayTimer() {
		m_dashDelayTriggered = true;
		m_dashDelayTimer = dashRefreshDelay;
	}

	void UpdateDashDelayTimer() {
		m_dashDelayTimer = m_dashDelayTimer - Time.deltaTime;

		if (m_dashDelayTimer <= 0.0f) {
			m_dashDelayTriggered = false;
		}
	}

	#endregion

	#region animation methods
	void Animating (float h, float v)
	{
		// Create a boolean that is true if either of the input axes is non-zero.
		bool walking = (h != 0f) || (v != 0f);

		if (walking) {
			if (m_isDashing && !GameManager.gm.DashBarManager().IsEmpty ()) {
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
