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
	}
	
	// Update is called once per frame
	void Update () {
		UpdateGameTimer ();
		UpdateUI ();
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

	public void UpdateDashTextMesh(string newText) {
		if (dashTextMesh.Text != newText) {
			dashTextMesh.Text = newText;
		}
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
	#endregion

	#region CameraShake
	public void ShakeCamera() {
		if (m_cameraShake) {
			m_cameraShake.ShakeCamera (0.45f, 0.02f);
		}
	}
	#endregion
}
