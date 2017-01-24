using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnCollision : MonoBehaviour {

	public string[] objectTags;

	void OnTriggerEnter(Collider other) {
		for (int index = 0; index < objectTags.Length; index++) {
			if (other.CompareTag(objectTags[index])) {
				Destroy(other.gameObject);
			}
		}
	}

}
