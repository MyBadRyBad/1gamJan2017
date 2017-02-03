using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryManager : MonoBehaviour {

	[Header("Prefabs")]
	//references to victory scene prefabs
	public Transform environmentPrefab;
	public Transform senpaiRedJacket;
	public Transform senpaiBlackJacket;
	public Transform senpaiBlackUniform;
	public Transform senpaiBlueJumpsuit;
	public Transform playerPrefab; 

	[Header("Camera Settings")]
	public GameObject mainCamera;
	public float cameraSpeed = 5.0f;

	// max amount of npcs that can fit on environment prefab
	private int m_maxSenpaiPerEnv = 73;

	// next position offsets
	private float m_environmentNextPositionX = 154.0f;
	private float m_senpaiNextPositionX = 2.0f;

	// refernce to the position of the last placed object
	private float m_lasObjPositionX;

	// reference to the List<T> found in PlayerStats
	private List<PlayerStats.SenpaiType> m_senpaiList;

	private bool m_moveCamera = false;

	#region Unity callbacks
	// inilialization
	void Start () {
		Test (1000);

		// reference of List in PlayerStats for ease
		m_senpaiList = PlayerStats.SenpaiTypeList;

		if (!mainCamera) {
			mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		}
			

		IncreaseCameraSpeed ();
		SetupEnvironment ();
		SetupCharacters ();

		StartCoroutine (SetMoveCamera (true, 1.0f));


	} 
	
	// Update is called once per frame
	void Update () {
		if (m_moveCamera) {
			// move the camera
			MoveCamera ();
			CameraReachedPlayer ();
		}
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
		float positionX = 0;

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

		// instantiate the player at the last object position minus offset
		Vector3 playerPos = new Vector3 (positionX - 0.5f, 0f, 0f);
		Quaternion playerRot = Quaternion.Euler (0f, 180f, 0f);
		Instantiate(playerPrefab, playerPos, playerRot);
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
	public void CameraReachedPlayer() {
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

	public void ShowUI() {

	}

	#endregion

	public void Test(int amount) {
		while (amount > 0) {
			PlayerStats.SenpaiType type;
			int random = amount % 4;

			if (random == 3) {
				type = PlayerStats.SenpaiType.SenpaiBlackJacket;
			} else if (random == 2) {
				type = PlayerStats.SenpaiType.SenpaiBlackUniform;
			} else if (random == 1) {
				type = PlayerStats.SenpaiType.SenpaiRedJacket;
			} else {
				type = PlayerStats.SenpaiType.SnepaiBlueJumpsuit;
			}

			PlayerStats.AddSenpaiType (type);

			amount--;
		}
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


}
