using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public enum PlayerState {
		Walking,
		Dashing,
		Idle,
		Diving
	};

	// movement speed variables
	public float defaultSpeed = 2.0f;
	public float dashSpeed = 4.0f;

	// dash variables
	public float dashDecreaseRate = 2.75f;
	public float dashIncreaseRate = 1.25f;
	public float dashRefreshDelay = 3.0f;

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

	private Vector3 m_jumpFowardVector;
	 private float m_jumpForwardVelocity = 100.0f;
	 private float m_jumpUpwardVelocity = 175.0f;
	[Header("Jump Velocities")]
	[SerializeField] private float m_hopForwardVelocity = 25.0f;
	[SerializeField] private float m_hopUpwardVelocity = 75.0f;
	[SerializeField] private float m_fallForwardVelocity = 130.0f;
	[SerializeField] private float m_fallUpwardVelocity = 125.0f;

	[Header("Wait Between Animations")]
	[SerializeField] private float m_preparationJumpAnimationWait = 0.6f;
	[SerializeField] private float m_jumpAnimationWait = 0.5f;
	[SerializeField] private float m_fallAnimationWait = 1.0f;
	[SerializeField] private float m_endAnimationWait = 0.5f;

	[Header("Animation Speeds")]
	[SerializeField] private float m_firstHopAnimationSpeed = 1.0f;
	[SerializeField] private float m_secondHopAnimationSpeed = 1.0f;
	[SerializeField] private float m_jumpFallAnimationSpeed = 2.0f;
	[SerializeField] private float m_jumpEndAnimationSpeed = 2.0f;


	[SerializeField]
	private PlayerState m_playerState;

	// flag to know when character is dashing
	private bool m_isDashing = false;
	private bool m_dashDelayTriggered = false;
	private bool m_dashButtonUpRefresh = true;

	// flag to trigger grab
	[SerializeField]
	private bool m_disableControls = false;
	private bool m_disableCollisions = false;

	#region Unity callbacks
	void Awake() {
		// Set up references.
		m_playerRigidbody = GetComponent <Rigidbody> ();
		m_sdMecanimController = GetComponent<QuerySDMecanimController> ();

	}
		
	void Start () {
		// set up timers
		m_dashDelayTimer = dashRefreshDelay;

		// setup start
		m_playerState = PlayerState.Idle;
	}

	void Update () {
		UpdateDash ();
	}

	void FixedUpdate ()
	{
		if (!m_disableControls) {
			// Store the input axes.
			float horizontal = Input.GetAxisRaw("Horizontal");
			float vertical = Input.GetAxisRaw("Vertical");

			// Move the player around the scene.
			Move (horizontal, vertical);

			// Execute dash if player holds down button
			if (m_dashButtonUpRefresh) {
				Dash (Input.GetButton ("Jump"));
			}

			if (Input.GetButtonUp("Jump")) {
				m_dashButtonUpRefresh = true;
			}


			// Animate the player.
			AnimateWalking (horizontal, vertical);

		}

		// Execute jump if necessary
		UpdateJumpVelocity();
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
		if (IsDashing () && !GameManager.gm.DashBarManager ().IsEmpty ()) {
			// update the dash text to animate
			GameManager.gm.UpdateDashTextMesh ("<j>Dash <size=26> (space bar) </s>");

			// refresh delay timer 
			SetupDashDelayTimer ();

			// decrease the dash bar 
			GameManager.gm.DashBarManager ().Decrease (dashDecreaseRate);

		} else if (!GameManager.gm.DashBarManager ().IsFull ()) {
			// update the dash text to stop animation
			GameManager.gm.UpdateDashTextMesh ("Dash <size=26> (space bar) </s>");

			//  increase the dash bar only if the delay is over.
			if (m_dashDelayTriggered) {
				UpdateDashDelayTimer ();
			} else {
				// increase the dash bar
				GameManager.gm.DashBarManager ().Increase (dashIncreaseRate);

			}
		} else if (GameManager.gm.DashBarManager ().IsEmpty ()) {
			// flag refresh so that if the player is holding down the dash button when the dash bar going empty,
			// the dash bar will continue to refill
			m_dashButtonUpRefresh = false;
			m_isDashing = false;
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
	void AnimateWalking (float h, float v)
	{
		// Create a boolean that is true if either of the input axes is non-zero.
		bool walking = (h != 0f) || (v != 0f);

		if (walking) {
			if (m_isDashing && !GameManager.gm.DashBarManager().IsEmpty ()) {
				m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN);
				m_playerState = PlayerState.Dashing;
			} else {
				m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_WALK);
				m_playerState = PlayerState.Walking;
			}
		} else {
			m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_IDLE);
			m_playerState = PlayerState.Idle;
		}
	}
		
	IEnumerator ExecuteDive() {
		m_disableControls = true;
		m_playerState = PlayerState.Diving;

		// remove any existing force
		m_playerRigidbody.velocity = Vector3.zero;
		m_playerRigidbody.angularVelocity = Vector3.zero;

		// move the player forward and animate the pre-jump
		TriggerJump (m_hopForwardVelocity, m_hopUpwardVelocity);
	//	m_sdMecanimController.ChangeAnimation (QuerySDMecanimController.QueryChanSDAnimationType.JUMP_PREPARE);
	//	m_sdMecanimController.ChangeAnimationWithSpeed(QuerySDMecanimController.QueryChanSDAnimationType.JUMP_PREPARE, m_firstHopAnimationSpeed);
	//	yield return new WaitForSeconds (m_preparationJumpAnimationWait);

		// animate the actual jump
		m_sdMecanimController.ChangeAnimationWithSpeed(QuerySDMecanimController.QueryChanSDAnimationType.JUMP_PREPARE, m_secondHopAnimationSpeed);
		yield return new WaitForSeconds(m_jumpAnimationWait);

		// move the player forward animate the laydown
		TriggerJump(m_fallForwardVelocity, m_fallUpwardVelocity);
		m_sdMecanimController.ChangeAnimationWithSpeed(QuerySDMecanimController.QueryChanSDAnimationType.JUMP_FALL, m_jumpFallAnimationSpeed);

		// wait a bit after the character reaches the floor
		yield return new WaitForSeconds(m_fallAnimationWait);

		// end the entire animation
		m_sdMecanimController.ChangeAnimationWithSpeed(QuerySDMecanimController.QueryChanSDAnimationType.JUMP_END, m_jumpEndAnimationSpeed);
		m_disableControls = false;

		m_playerState = PlayerState.Idle;
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
		

	#endregion

	public void Dive () {
		// stop any existing routine
		StopCoroutine(ExecuteDive());

		// start coroutine
		StartCoroutine(ExecuteDive());
	}

	#region getter methods
	public bool IsIdle() {
		return m_playerState == PlayerState.Idle;
	}

	public bool IsWalking() {
		return m_playerState == PlayerState.Walking;
	}

	public bool IsDashing() {
		return m_playerState == PlayerState.Dashing;
	}

	public bool IsDiving() {
		return m_playerState == PlayerState.Diving;
	}

	public void SetEnableControls(bool enable) {
		m_disableControls = !enable;
	}

	public void SetEnableCollision(bool enable) {
		m_disableCollisions = !enable;
	}

	public bool CollisionEnabled() {
		return !m_disableCollisions;
	}
	#endregion
}
