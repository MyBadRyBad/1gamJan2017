using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	// static reference for game manager
	public static GameManager gm;

	// start time for the game timer
	public float startTime = 30.0f;

	// UI elements
	public Text timerText;
	public Text pointsText;

	// game timer
	private float currentTime;

	private int points;

	#region Unity callback
	// Use this for initialization
	void Awake() {
		// setup reference to game manager
		if (gm == null)
			gm = this.GetComponent<GameManager>();
	}

	void Start () {
		currentTime = startTime;
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
			timerText.text = currentTime.ToString ("F2");
		}

		if (pointsText) {
			pointsText.text = points.ToString ();
		}
	}
	#endregion


	#region game logic
	public void AddPoints(int number) {
		points = points + number;
	}

	public void AddTime(float time) {
		currentTime = currentTime + time;
	}
	#endregion
}
