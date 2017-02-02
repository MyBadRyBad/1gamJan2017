using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	// static reference for game manager
	public static GameManager gm;

	[Header("Camera")]
	// refernce to the mainCamera
	public Camera mainCamera;


	[Header("UI Canvas")]
	// canvas
	public Canvas m_canvas;

	// reference to dash super text
	public SuperTextMesh dashTextMesh;

	// reference to the dash bar
	public Image dashBar;

	// UI elements
	public SuperTextMesh timerText;
	public SuperTextMesh pointsText;
	public SuperTextMesh senpaiGetText;

	[Header("Add Points")]
	public SuperTextMesh addPointPrefab;
	public SuperTextMesh addTimePrefab;
	public Transform addPointSpawnPosition;
	public Transform addTimeSpawnPosition;

	[Header("Game Logic")]
	// start time for the game timer
	public float startTime = 30.0f;


	// references 
	private DashBarManager m_dashBarManager;
	private CameraShake m_cameraShake;
	private GameObject m_player;

	// game timer
	private float currentTime;

	// global points
	private int m_points;


	private bool m_GameEnabled = false;

	#region Unity callback
	// Use this for initialization
	void Awake() {
		// setup reference to game manager
		if (gm == null)
			gm = this.GetComponent<GameManager>();

	}

	void Start () {
		currentTime = startTime;

		// get a reference to the DashBarManager
		if (dashBar != null) {
			m_dashBarManager = dashBar.GetComponent<DashBarManager> ();
		}

		if (mainCamera) {
			m_cameraShake = mainCamera.GetComponent<CameraShake> ();
		}

		m_player = GameObject.FindGameObjectWithTag ("Player");

		GameManager.gm.DisableGame ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_GameEnabled) {
			UpdateGameTimer ();
			UpdateUI ();
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

	#endregion

	#region UI methods
	void UpdateUI() {
		if (timerText) {
			if (currentTime <= 10.0f) {
				timerText.Text = "<c=danger>" + currentTime.ToString ("F1");
				timerText.size = 60.0f;
			} else {
				timerText.Text = "<c=normal>" + currentTime.ToString ("F1");
				timerText.size = 40.0f;
			}
		}

		if (pointsText) {
			pointsText.Text = m_points.ToString ();
		}
	}

	public void ShowSenpaiGetText() {
		senpaiGetText.Text = "<drawAnim=Appear><c=watermelon><w=sassy>Senpai Get!!!";

		StartCoroutine (HideSenpaiText (1.0f));

	}

	public void UpdateDashTextMesh(string newText) {
		if (dashTextMesh.Text != newText) {
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
		timerText.GetComponent<Animator> ().SetTrigger ("ScaleBounce");
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
		currentTime = currentTime + time;
	}
		
	#endregion

	#region getter/setter methods

	public DashBarManager DashBarManager() {
		return m_dashBarManager;
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
