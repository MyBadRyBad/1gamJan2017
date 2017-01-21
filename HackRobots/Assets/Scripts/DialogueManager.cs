using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	public GameObject dialogueBox;
	public Text dialogueText;

	public bool dialogueActive;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (dialogueActive && Input.GetKeyDown(KeyCode.Space)) {

			// deactivate box
			dialogueBox.SetActive (false);
			dialogueActive = false;
		}
	}


	// Show Dialogue Box on screen
	public void ShowDialogueBox(string dialogue) {
		dialogueActive = true;
		dialogueBox.SetActive (true);
		dialogueText.text = dialogue;
	}
}
