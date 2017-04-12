using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public GameObject player;

	public Weapon weapon;
	public float bulletSpeed;
	public float bulletKnockBack = 1;
	public float bulletSize; //possibly scale this with damage
	public float bulletDelay; //at 0, the gun fires every frame
	public float bulletDamage;
	protected float currTime = 0;

	public float movementSpeed;
	public float turnSpeed;
	public float knockBackResistance;
	public float MAX_HEALTH;
	public float current_health;

	protected float knockBackDuration;
	protected Vector3 knockBackDirection;
	void Start () {
		knockBackDuration = 0;
		current_health = MAX_HEALTH;
		player = GameObject.Find ("Player");
	}

	public virtual void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("Bullet")) {
			float damageInflicted = other.GetComponent<BulletScript> ().getDamage();
			float bulletKnockBack = other.GetComponent<BulletScript> ().getKnockBack();
			Vector3 direction = other.GetComponent<BulletScript> ().getDirection();
			Destroy (other.gameObject);
			inflictDamage (damageInflicted);

			knockBackDirection = transform.worldToLocalMatrix * (bulletKnockBack/knockBackResistance * direction);
			knockBackDuration = Time.time + 0.1f;
		}
	}

	public virtual void inflictDamage (float amount) {
		current_health -= amount;
		if (current_health <= 0.0f) {
			Destroy (gameObject);
		}
	}
}
