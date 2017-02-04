using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(MainMenuManager))]
public class MainMenuEditor : Editor { // extend the Editor class

	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();

		// get a reference to the GameManager script on this target gameObject
		MainMenuManager myGM = (MainMenuManager)target;

		// add a custom button to the Inspector component
		if(GUILayout.Button("Reset High score"))
		{
			PlayerPrefs.SetInt (Globals.PlayerPrefValues.HIGHEST_SCORE, 0);
		}

		if (GUILayout.Button ("Reset Total Collected")) {
			PlayerPrefs.SetInt (Globals.PlayerPrefValues.TOTAL_POINTS, 0);
		}

	}
}
