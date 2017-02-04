using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	// static reference for game manager
	public static GameManager gm;

	// reference to the 

	[Header("Camera")]
	// refernce to the mainCamera
	public Camera mainCamera;


	[Header("UI Canvas")]
	// canvas
	public Canvas m_canvas;

	// reference to dash super text
	public SuperTextMesh dashTextMesh;

	// reference to the dash bar
	public Transform dashBar;
	public Image dashBarImage;

	// UI elements
	public SuperTextMesh timerText;
	public SuperTextMesh pointsText;
	public SuperTextMesh senpaiGetText;
	public SuperTextMesh instructionsText;
	public SuperTextMesh loadingText;

	[Header("Add Points")]
	public SuperTextMesh addPointPrefab;
	public SuperTextMesh addTimePrefab;
	public Transform addPointSpawnPosition;
	public Transform addTimeSpawnPosition;

	[Header("Game Logic")]
	// start time for the game timer
	public float startTime = 30.0f;

	[Header("Audio Clips")]
	public AudioClip blipClip;
	public AudioClip hornClip;

	// references 
	private DashBarManager m_dashBarManager;
	private CameraShake m_cameraShake;
	private GameObject m_player;
	private AudioSource m_audioSource;

	// game timer
	private float currentTime;

	// global points
	private int m_points;

	// game enabled trigger
	private bool m_GameEnabled = false;

	#region Unity callback
	// Use this for initialization
	void Awake() {
		
		// setup reference to game manager
		if (gm == null) {
			gm = this.GetComponent<GameManager> ();
		}
		

		m_audioSource = GetComponent<AudioSource> ();

	}

	void Start () {
		currentTime = startTime;

		// get a reference to the DashBarManager
		if (dashBarImage != null) {
			m_dashBarManager = dashBarImage.GetComponent<DashBarManager> ();
		}

		if (mainCamera) {
			m_cameraShake = mainCamera.GetComponent<CameraShake> ();
		}
		// find player
		m_player = GameObject.FindGameObjectWithTag ("Player");

		// setup game
		GameManager.gm.DisableGame ();
		PlayerStats.Clear ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_GameEnabled) {
			UpdateGameTimer ();
			UpdateUI ();

			if (currentTime <= 0.0f) {
				EndGame ();
			}
		}
	}

	#endregion

	#region manager logic methods
	void UpdateGameTimer() {
		currentTime -= Time.deltaTime;

		if (currentTime < 0.0f) {
			currentTime = 0.0f;
		}
	}

	public void ActivateScene() {
		m_audioSource.Play ();
		dashBar.gameObject.SetActive (true);
		timerText.Text = "<c=normal>30.0";
		dashTextMesh.Text = "Dash <size=26> (space bar) </s>";
		pointsText.Text = "0";
		ShowInstructionsText ();
		StartCoroutine (StartGame (4.5f));
	}
		

	#endregion

	#region UI methods
	void UpdateUI() {
		if (timerText) {
			if (currentTime <= 10.0f) {
				string timeString = currentTime.ToString ("F1");
				timerText.Text = "<c=danger>" + timeString;
				timerText.size = 60.0f;

				PlayCountDownAudio (timeString);

			} else {
				string timeString = currentTime.ToString ("F1");
				timerText.Text = "<c=normal>" + timeString;
				timerText.size = 40.0f;
			}
		}

		if (pointsText) {
			pointsText.Text = m_points.ToString ();
		}
	}

	public void ShowInstructionsText() {
		instructionsText.Text = " ";
		instructionsText.Text = "Collect<br>all<br><j>Senpais!";
	}

	public void ShowSenpaiGetText() {
		senpaiGetText.Text = "<drawAnim=Appear><c=watermelon><w=sassy>Senpai Get!!!";

		StartCoroutine (HideSenpaiText (1.0f));

	}

	public void UpdateDashTextMesh(string newText) {
		if (dashBar && dashTextMesh && dashTextMesh.Text != newText) {
			dashTextMesh.Text = newText;
		}
	}

	public void CreateAddPointText() {
		//Instantiate the text Mesh
		SuperTextMesh textMesh = Instantiate (addPointPrefab, addPointSpawnPosition.position, addPointSpawnPosition.rotation) as SuperTextMesh;

		// update the rect transform
		textMesh.GetComponent<RectTransform> ().anchoredPosition = addPointSpawnPosition.GetComponent<RectTransform> ().anchoredPosition;

		// set the parent to the canvas
		textMesh.transform.SetParent (m_canvas.transform, false);
	}

	public void CreateAddTimeText() {
		// Instantiate the text mesh
		SuperTextMesh textMesh = Instantiate (addTimePrefab, addTimeSpawnPosition.position, addTimeSpawnPosition.rotation) as SuperTextMesh;

		// update the rect transform
		textMesh.GetComponent<RectTransform> ().anchoredPosition = addTimeSpawnPosition.GetComponent<RectTransform> ().anchoredPosition;

		// set the parent to the canvas
		textMesh.transform.SetParent (m_canvas.transform, false);
	}

	public void AnimatePointsTextMesh() {
		pointsText.GetComponent<Animator> ().SetTrigger ("ScaleBounce");
	}

	public void AnimateTimeTextMesh() {
		if (currentTime > 0.0f) {
			timerText.GetComponent<Animator> ().SetTrigger ("ScaleBounce");
		}
	}

	IEnumerator HideSenpaiText(float delay) {
		yield return new WaitForSeconds (delay);
		senpaiGetText.UnRead ();
	}
		
	#endregion


	#region game logic
	public void AddPoints(int number) {
		m_points = m_points + number;
	}

	public void AddTime(float time) {
		if (currentTime > 0.0f) {
			currentTime = currentTime + time;
		}
	}

	public void EndGame() {
		DisableGame ();
		instructionsText.Text = "Times up!";
		StartCoroutine (GoToLevel ("Victory", 8.0f));


	}

	public void PlayCountDownAudio(string time) {
		if (time == "1.0" || time == "2.0" || time == "3.0") {
			PlayAudio (blipClip);
		} else if (time == "0.0") {
			PlayAudio (hornClip);
		}
	}

	public void PlayAudio(AudioClip clip) {
		if (m_audioSource && clip && !m_audioSource.isPlaying) {
			m_audioSource.PlayOneShot (clip);
		}
	}

	public void StoreSenpaiType(PlayerStats.SenpaiType senpaiType) {
		PlayerStats.AddSenpaiType (senpaiType);
	}
		
	IEnumerator GoToLevel(string levelName, float delay) {
		yield return new WaitForSeconds (3.0f);

		loadingText.Text = "<w>Loading";
		senpaiGetText.Text = " ";

		yield return new WaitForSeconds (delay - 3.0f);
		SceneTransitionManager.manager.TransitionToScene (3);
	}

	IEnumerator StartGame(float delay) {
		yield return new WaitForSeconds (delay);

		GameManager.gm.EnableGame ();
		instructionsText.Text = " ";
	}

	#endregion

	#region Helper

	public DashBarManager DashBarManager() {
		return m_dashBarManager;
	}

	public bool GameEnabled() {
		return m_GameEnabled;
	}

	public void EnableGame() {
		PlayerController playerController = m_player.GetComponent<PlayerController> ();
		playerController.SetEnableControls (true);
		playerController.SetEnableCollision (true);
		m_GameEnabled = true;

	} 

	public void DisableGame() {
		PlayerController playerController = m_player.GetComponent<PlayerController> ();
		playerController.SetEnableControls (false);
		playerController.SetEnableCollision (false);
		m_GameEnabled = false;
	}

	#endregion

	#region CameraShake
	public void ShakeCamera() {
		if (m_cameraShake) {
			m_cameraShake.ShakeCamera (0.45f, 0.02f);
		}
	}
	#endregion

}
