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

	// grab timer
	private float m_grabDelay = 2.0f;
	private float m_grabTimer;
	private bool m_triggerJump = false;

	[SerializeField]
	private Vector3 m_jumpFowardVector;
	private float m_jumpForwardVelocity = 100.0f;
	private float m_jumpUpwardVelocity = 175.0f;
	private float m_preparationJumpForwardVelocity = 100.0f;
	private float m_preparationJumpUpwardVelocity = 75.0f;
	private float m_fallForwardVelocity = 100.0f;
	private float m_fallUpwardVelocity = 175.0f;

	private float m_preparationJumpAnimationWait = 0.6f;
	public float m_jumpAnimationWait = 0.5f;
	public float m_fallAnimationWait = 1.0f;
	public float m_endAnimationWait = 1.0f;

	// flag to know when character is dashing
	private bool m_isDashing = false;
	private bool m_dashDelayTriggered = false;

	// flag to trigger grab
	[SerializeField]
	private bool m_disableControls = false;


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

		if (Input.GetKeyDown (KeyCode.Z)) {
			StartCoroutine(ExecuteGrab());
		}
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

		// Execute jump if necessary
		UpdateJumpVelocity();

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


	void UpdateGrabTimer() {
		if (m_disableControls) {
			m_grabTimer -= Time.deltaTime;

			if (m_grabTimer <= 0.0f) {
				m_disableControls = false;
			}
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
		if (m_disableControls) {

		} else {
			AnimateWalking (h, v);
		}
	}
		
	IEnumerator ExecuteGrab() {
		m_disableControls = true;

		// move the player forward and animate the pre-jump
		TriggerJump (m_preparationJumpForwardVelocity, m_preparationJumpUpwardVelocity);
		m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.JUMP_PREPARE);
		yield return new WaitForSeconds (m_preparationJumpAnimationWait);

		// animate the actual jump
		m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.JUMP_PREPARE);
		yield return new WaitForSeconds(m_jumpAnimationWait);

		// move the player forward animate the laydown
		TriggerJump(m_fallForwardVelocity, m_fallUpwardVelocity);
		m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.JUMP_FALL);

		// wait a bit after the character reaches the floor
		yield return new WaitForSeconds(m_fallAnimationWait);

		// end the entire animation
		m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.JUMP_END);
		m_disableControls = false;
	}

	void TriggerJump (float forwardVelocity, float upwardVelocity) {
		m_jumpForwardVelocity = forwardVelocity;
		m_jumpUpwardVelocity = upwardVelocity;
		m_triggerJump = true;
	}

	void UpdateJumpVelocity() {
		if (m_triggerJump) {
			m_triggerJump = false;

			Vector3 forwardForce = transform.forward * m_jumpForwardVelocity;
			Vector3 upwardForce = transform.up * m_jumpUpwardVelocity;
			m_playerRigidbody.AddForce(forwardForce + upwardForce);
		}
	} 

	void AnimateWalking(float h, float v) {
		if (!m_disableControls) {
			// Create a boolean that is true if either of the input axes is non-zero.
			bool walking = (h != 0f) || (v != 0f);

			if (walking) {
				if (m_isDashing && !GameManager.gm.DashBarManager().IsEmpty ()) {
					m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN);
				} else {
					m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_WALK);
				}
			} else {
				m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_IDLE);
			}
		}
	}
	#endregion
}
