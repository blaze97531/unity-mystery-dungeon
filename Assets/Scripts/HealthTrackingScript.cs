using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTrackingScript : MonoBehaviour {

	public float MAX_HEALTH;
	public float current_health;

	// Use this for initialization
	void Start () {
		current_health = MAX_HEALTH;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("Bullet")) {
			float damageInflicted = other.GetComponent<BulletScript> ().getDamage();
			float bulletKnockBack = other.GetComponent<BulletScript> ().getKnockBack();
			Vector3 direction = other.GetComponent<BulletScript> ().getDirection();
			//float knockBackResistance = GameObject.GetComponent<BulletScript> ().getknockBackResistance();
			Destroy (other.gameObject);
			inflictDamage (damageInflicted);
			transform.Translate (transform.worldToLocalMatrix * (bulletKnockBack/1000 * direction));
		}
	}

	public void inflictDamage (float amount) {
		current_health -= amount;
		if (current_health <= 0.0f) {
			Destroy (gameObject);
		}
	}
}
