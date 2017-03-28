using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTrackingScript : MonoBehaviour {

	public float MAX_HEALTH;
	private float current_health;

	// Use this for initialization
	void Start () {
		current_health = MAX_HEALTH;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void inflictDamage (float amount) {
		current_health -= amount;
		if (current_health <= 0.0f) {
			Destroy (gameObject);
		}
	}
		
}
