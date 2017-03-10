using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBulletCollisionScript : MonoBehaviour {

	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Bullet")) {
			Destroy (other.gameObject);
		}
	}
}
