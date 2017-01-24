using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour {

	public GameObject[] prefabs;

	public Vector3 direction; 

	public float spawnTimeMin = 3.0f;
	public float spawnTimeMax = 3.0f;

	public Transform spawnPoint;

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
		int prefabsIndex = Random.Range (0, prefabs.Length);

		// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
		GameObject vehicle = Instantiate (prefabs[prefabsIndex], spawnPoint.position, spawnPoint.rotation) as GameObject;

		// place this vehicle inside the spawner object for organizational purposes
		vehicle.transform.parent = gameObject.transform;

		// setup vehicle movement for newly instantiated vehicle
		VehicleMovement vehicleMovement = vehicle.GetComponent<VehicleMovement> ();
		if (vehicleMovement) {
			vehicleMovement.direction = direction;

			// vehicle is moving down, so rotate it to face the opposite direction
			if (direction.z < 0) {
				vehicleMovement.rotation = new Vector3 (vehicleMovement.rotation.x, vehicleMovement.rotation.y + 180, vehicleMovement.rotation.z);
			}
		}
			
		Invoke ("Spawn", Random.Range(spawnTimeMin, spawnTimeMax));
	}
	#endregion
}
