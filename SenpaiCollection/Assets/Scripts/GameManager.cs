using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	// static reference for game manager
	public static GameManager gm;

	// refernce to the mainCamera
	public Camera mainCamera;

	// reference to dash super text
	public SuperTextMesh dashTextMesh;

	// reference to the dash bar
	public Image dashBar;

	// UI elements
	public SuperTextMesh timerText;
	public SuperTextMesh pointsText;
	public SuperTextMesh senpaiGetText;

	// start time for the game timer
	public float startTime = 30.0f;

	// game timer
	private float currentTime;

	// global points
	private int m_points;

	// reference to the DashBarManager
	private DashBarManager m_dashBarManager;

	// refernce to the CameraShake script
	private CameraShake m_cameraShake;

	// reference to the player
	private GameObject m_player;

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
		m_player.GetComponent<PlayerController> ().SetEnableControls (false);
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
	}

	#endregion

	#region UI methods
	void UpdateUI() {
		if (timerText) {
			timerText.Text = currentTime.ToString ("F1");
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
		m_GameEnabled = true;
		m_player.GetComponent<PlayerController> ().SetEnableControls (true);

	} 

	public void DisableGame() {
		m_GameEnabled = false;
		m_player.GetComponent<PlayerController> ().SetEnableControls (false);
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
