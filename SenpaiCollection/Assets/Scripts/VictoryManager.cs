using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

		Debug.Log ("Count: " + PlayerStats.SenpaiTypeList.Count);
		for (int index = 0; index < PlayerStats.SenpaiTypeList.Count; index++) {
			Debug.Log (PlayerStats.SenpaiTypeList[index]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
