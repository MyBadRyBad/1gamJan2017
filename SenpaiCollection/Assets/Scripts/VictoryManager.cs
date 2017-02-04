using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour {

	public static VictoryManager manager;

	[Header("Prefabs")]
	//references to victory scene prefabs
	public Transform environmentPrefab;
	public Transform senpaiRedJacket;
	public Transform senpaiBlackJacket;
	public Transform senpaiBlackUniform;
	public Transform senpaiBlueJumpsuit;

	[Header("Player Object")]
	public Transform player;

	[Header("UI Elements")]
	public SuperTextMesh mainText;
	public SuperTextMesh pointsText;
	public SuperTextMesh replayText;
	public SuperTextMesh mainMenuText;
	public SuperTextMesh newHighscoreText;

	public Button replayButton;
	public Button mainMenuButton;

	[Header("Camera Settings")]
	public GameObject mainCamera;
	public float cameraSpeed = 5.0f;

	// max amount of npcs that can fit on environment prefab
	private int m_maxSenpaiPerEnv = 73;

	// points to display
	private float m_points = 0f;

	// next position offsets
	private float m_environmentOffsetX = 70.0f;
	private float m_environmentNextPositionX = 154.0f;
	private float m_senpaiNextPositionX = 2.0f;

	// refernce to the position of the last placed object
	private float m_lasObjPositionX;

	// reference to the List<T> found in PlayerStats
	private List<PlayerStats.SenpaiType> m_senpaiList;

	// event flags
	private bool m_moveCamera = false;
	private bool m_animateCount = false;
	private bool m_isNewHighscore = false;

	#region Unity callbacks

	void Awake() {
		if (manager == null) {
			Debug.Log ("allocating manager");
			manager = GetComponent<VictoryManager> ();
		}
	}

	// initialization
	void Start () {

		// reference of List in PlayerStats for ease
		m_senpaiList = PlayerStats.SenpaiTypeList;

		if (!mainCamera) {
			mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		}
			
		IncreaseCameraSpeed ();
		SetupEnvironment ();
		SetupCharacters ();
	} 
	
	// Update is called once per frame
	void Update () {
		if (m_moveCamera) {
			// move the camera
			MoveCamera ();
			CameraReachedPlayer ();
		}

		UpdatePointsText ();
	}

	public void ActivateScene() {
		
		StartCoroutine (SetMoveCamera (true, 1.0f));
	}

	#endregion

	#region setup
	void IncreaseCameraSpeed() {
		// increase camera speed
		float factor = (PlayerStats.SenpaiTypeList.Count / 50f) + 1f;
		cameraSpeed = cameraSpeed * factor;
		Debug.Log ("newCameraSpeed: " + cameraSpeed);
	}

	void SetupEnvironment() {
		int numberOfEnvironment = PlayerStats.SenpaiTypeList.Count / 73;
		float positionX = m_environmentOffsetX;

		while (numberOfEnvironment > 0) {
			positionX = positionX + m_environmentNextPositionX;
			Vector3 position = new Vector3 (positionX, 0.0f, 0.0f);

			Instantiate (environmentPrefab, position, Quaternion.identity);

			numberOfEnvironment--;
		}
	}

	void SetupCharacters() {
		float positionX = 0;

		// setup the senpais
		for (int index = 0; index < m_senpaiList.Count; index++) {
			// place senpai and rotate 180 to face the camera
			Vector3 pos = new Vector3 (positionX, 0f, 0f);
			Quaternion rotation = Quaternion.Euler (0f, 180f, 0f);

			// instantiate
			Instantiate(PrefabForSenpaiType(m_senpaiList[index]), pos, rotation);

			// update position for next senpai
			positionX = positionX + m_senpaiNextPositionX;
		}

		// positionX is now the lastObj position
		m_lasObjPositionX = positionX;

		// place the  player at the last object position minus offset
		Vector3 playerPos = new Vector3 (positionX - 0.5f, 0f, 0f);
		Quaternion playerRot = Quaternion.Euler (0f, 160f, 0f);
		player.position = playerPos;
		player.rotation = playerRot;

		// set the player's animation to idle
		player.gameObject.GetComponent<PlayerVictoryBehavior>().Idle();
	}
		
	#endregion

	#region camera movement
	IEnumerator SetMoveCamera(bool move, float delay) {
		yield return new WaitForSeconds (delay);
		m_moveCamera = move;
	}

	void MoveCamera() {
		mainCamera.transform.position += new Vector3 (cameraSpeed * Time.deltaTime, 0f, 0f);
	}

	#endregion

	#region UI Update
	void CameraReachedPlayer() {
		// if the camera reaches the player
		if (mainCamera.transform.position.x >= m_lasObjPositionX) {
			// fix the main camera's position if it went over the player's position
			mainCamera.transform.position = new Vector3(m_lasObjPositionX, 
														mainCamera.transform.position.y,
														mainCamera.transform.position.z);
			
			//stop the camera
			m_moveCamera = false;

			// trigger points UI
			ShowUI();
		}
	}

	void ShowUI() {
		mainText.Text = "Senpais<br>Collected";

		Invoke ("ToggleAnimateCount", 1.0f);

	}
		
	void UpdatePointsText() {
		if (m_animateCount && m_points < PlayerStats.SenpaiTypeList.Count) {
			float speed = 20.0f + (PlayerStats.SenpaiTypeList.Count / 10);
			m_points += (Time.deltaTime * speed);

			if (m_points >= PlayerStats.SenpaiTypeList.Count) {
				m_points = PlayerStats.SenpaiTypeList.Count;

				m_animateCount = false;
				// animate player and show replay and exit button
				EnableReplayButtons ();
			}
			pointsText.Text = m_points.ToString ("f0");
		
		} 
		// player collected nothing
		else if (m_animateCount && PlayerStats.SenpaiTypeList.Count == 0) {
			Debug.Log ("enable");
			m_animateCount = false;
			EnableReplayButtons ();
		}

	}

	void EnableReplayButtons() {
		// show high score text if necessary
		ShowNewHighscore();

		// save scores
		SaveScores();

		// first animate the player accordingly
		AnimatePlayer();

		// enable replaybuttons
		replayButton.gameObject.SetActive(true);
		mainMenuButton.gameObject.SetActive (true);
	}

	void AnimatePlayer() {

		// show lose
		if (m_points < 1) {
			player.GetComponent<PlayerVictoryBehavior> ().Lose ();
		} else if (m_isNewHighscore) { // show new high score animation
			player.GetComponent<PlayerVictoryBehavior> ().WinHighScore ();
		} else { // show normal win
			player.GetComponent<PlayerVictoryBehavior> ().Win ();
		}
			
	}
	#endregion


	#region button events
	public void AnimateReplayButton(bool animate) {
		if (animate) {
			replayText.Text = "<w>Replay";
		} else {
			replayText.Text = "Replay";
		}
	}
		
	public void AnimateMainMenuButton(bool animate) {
		if (animate) {
			mainMenuText.Text = "<w>Main Menu";
		} else {
			mainMenuText.Text = "Main Menu";
		}
	}
		

	public void LoadScene(int scene) {
		SceneTransitionManager.manager.TransitionToScene (scene);
	}

	#endregion

	#region Helper methods
	void ToggleAnimateCount() {
		pointsText.Text = "0";
		m_animateCount = !m_animateCount;
	}
		
	Transform PrefabForSenpaiType(PlayerStats.SenpaiType senpaiType) {
		if (senpaiType == PlayerStats.SenpaiType.SenpaiBlackJacket) {
			return senpaiBlackJacket;
		} else if (senpaiType == PlayerStats.SenpaiType.SenpaiBlackUniform) {
			return senpaiBlackUniform;
		} else if (senpaiType == PlayerStats.SenpaiType.SnepaiBlueJumpsuit) {
			return senpaiBlueJumpsuit;
		} else {
			return senpaiRedJacket;
		}
	}

	public void ShowNewHighscore() {
		int highest = PlayerPrefs.GetInt (Globals.PlayerPrefValues.HIGHEST_SCORE, 0);
		Debug.Log ("highest score: " + highest);
		Debug.Log ("current score: " + m_points);
		if (highest < Mathf.RoundToInt(m_points)) {
			Debug.Log ("is new high score");
			newHighscoreText.Text = "<c=watermelon>New Highscore!";
			m_isNewHighscore = true;
		}

	}

	public void SaveScores() {
		int currentTotal = PlayerPrefs.GetInt (Globals.PlayerPrefValues.TOTAL_POINTS, 0);
		int highest = PlayerPrefs.GetInt (Globals.PlayerPrefValues.HIGHEST_SCORE, 0);

		currentTotal = currentTotal + Mathf.RoundToInt(m_points);

		if (highest < Mathf.RoundToInt(m_points)) {
			highest = Mathf.RoundToInt(m_points);
		}

		PlayerPrefs.SetInt (Globals.PlayerPrefValues.TOTAL_POINTS, currentTotal);
		PlayerPrefs.SetInt (Globals.PlayerPrefValues.HIGHEST_SCORE, highest);
		PlayerPrefs.Save ();

	}

	#endregion


}
