using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsManager : MonoBehaviour {

	public SuperTextMesh instructionsText;

	float thirdTextTimer = 4.0f;
	// Use this for initialization
	void Start () {
		instructionsText.Text = "Collect<br>all<br><j>Senpais!";
		StartCoroutine (EnableGame (5.0f));
	
	}
	
	IEnumerator EnableGame(float delay) {
		yield return new WaitForSeconds (delay);

		GameManager.gm.EnableGame ();
		instructionsText.Text = " ";
	}
}
