using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
		
	[Header("Object references")]
	public Transform player;
	public Transform[] backgroundObjects;
	public Transform[] senpaiObjects;

	[Header("Buttons")]
	public Button playButton;
	public Button creditsButton;
	public Button backButton;


	// references to the text meshes of the buttons
	[Header("UI Text Meshes")]
	public SuperTextMesh mainText;
	public SuperTextMesh playButtonText;
	public SuperTextMesh creditsButtonText;
	public SuperTextMesh backButtonText;
	public SuperTextMesh loadingText;

	[Header("Credits lines")]
	public string[] creditsLines;

	[Header("Background Movement Speed")]
	public float backgroundObjSpeed = 2.0f;

	[Header("Senpai Movement Speed")]
	public float senpaiMovementSpeed = 2.0f;

	[Header("Teleport pos.x Locations")]

	// x positions where backgrounds will teleport in an endless loop
	public float teleportLocationX;
	public float teleportDestinationX;

	private int m_creditsLineIndex = 0;

	// credits flags and timers
	private float m_creditTimerMax = 10.0f;
	private float m_creditTimerCurrent = 0.0f;
	private bool m_showCredits = false;

	#region unity Callback
	// Use this for initialization
	void Start () {
		if (player) {
			AnimatePlayerRunning ();
		}
	}
		
	
	// Update is called once per frame
	void Update () {
		AnimateObjects (backgroundObjects, backgroundObjSpeed);
		AnimateObjects (senpaiObjects, senpaiMovementSpeed);
		UpdateCredits ();
	}

	#endregion

	#region animation methods
	void AnimateObjects(Transform[] objs, float speed) {
		for (int index = 0; index < objs.Length; index++) {
			// move object
			Transform obj = objs [index];
			obj.position -= new Vector3 (speed * Time.deltaTime, 0f, 0f);

			// if object reaches end of level, teleport to the other side of the level
			if (obj.position.x <= teleportLocationX) {
				obj.position = new Vector3 (teleportDestinationX, obj.position.y, obj.position.z);
			}
		}
	}

	void AnimatePlayerRunning() {
		player.GetComponent<QuerySDMecanimController> ().ChangeAnimationWithSpeed (QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN, 1.0f);
	}
	#endregion

	#region button methods
	public void LoadLevel(string level) {
		
		// display buttons
		playButton.gameObject.SetActive(false);
		backButton.gameObject.SetActive (false);
		creditsButton.gameObject.SetActive (false);

		// set loading text
		loadingText.Text = "<w>Loading";

		// load scene
		SceneManager.LoadSceneAsync (level);
	}

	public void ShowCredits(bool show) {
		if (show) {
			// show/hide necessary buttons
			playButton.gameObject.SetActive(false);
			creditsButton.gameObject.SetActive (false);
			backButton.gameObject.SetActive (true);

			// setup/show text
			mainText.size = 20f;
			mainText.readDelay = 0.001f;

			// trigger on credits
			m_showCredits = true;
			m_creditTimerCurrent = 0.0f;

		} else {

			// trigger off credits
			m_showCredits = false;

			// show/hide necessary buttons
			playButton.gameObject.SetActive(true);
			creditsButton.gameObject.SetActive (true);
			backButton.gameObject.SetActive (false);

			// setup/show text
			mainText.readDelay = 0.1f;
			mainText.size = 64f;
			mainText.Text = "<size=28>Watashi No</size><br><c=tak>Senpai<br>Collection";

		}
	}

	public void AnimatePlayButton(bool animate) {
		if (animate) {
			playButtonText.Text = "<w>Play";
		} else {
			playButtonText.Text = "Play";
		}
	}

	public void AnimateCreditsButton(bool animate) {
		if (animate) {
			creditsButtonText.Text = "<w>Credits";
		} else {
			creditsButtonText.Text = "Credits";
		}
	}

	public void AnimateBackButton(bool animate) {
		if (animate) {
			backButtonText.Text = "<w>Back";
		} else {
			backButtonText.Text = "Back";
		}
	}
	#endregion

	#region credits method
	void UpdateCredits() {
		if (m_showCredits) {
			if (m_creditTimerCurrent <= 0.0f) {
				m_creditTimerCurrent = m_creditTimerMax;

				mainText.readDelay = 0.001f;
				mainText.Text = "";

				mainText.Text = creditsLines [m_creditsLineIndex];
				m_creditsLineIndex++; 
				m_creditsLineIndex %= creditsLines.Length; 
			} else {
				m_creditTimerCurrent -= Time.deltaTime;
			}

		} 
	
	}
	#endregion
}

