using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

	public GameObject[] prefabs;
	public float spawnTimeMin = 3.0f;
	public float spawnTimeMax = 3.0f;
	public Transform[] spawnPoints;

	public Vector3 direction;

	#region Unity callbacks
	// Use this for initialization
	void Start () {
		Invoke ("Spawn", Random.Range(spawnTimeMin, spawnTimeMax));
	}

	#endregion
		
	#region spawn methods

	void Spawn ()
	{
		// If the player has no health left...
	//	if(playerHealth.currentHealth <= 0f)
	//	{
			// ... exit the function.
	//		return;
	//	}

		// Find a random index between zero and one less than the number of spawn points.
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);
		int prefabsIndex = Random.Range (0, prefabs.Length);

		// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
		Instantiate (prefabs[prefabsIndex], spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);

		Invoke ("Spawn", Random.Range(spawnTimeMin, spawnTimeMax));
	}
	#endregion

}
