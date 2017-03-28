using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBulletCollisionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("Bullet")) {
			float damageInflicted = other.GetComponent<BulletScript> ().getDamage();
			Destroy (other.gameObject);
			HealthTrackingScript thisObjectsHealth = gameObject.GetComponent<HealthTrackingScript> ();
			thisObjectsHealth.inflictDamage (damageInflicted);
		}
	}
}
