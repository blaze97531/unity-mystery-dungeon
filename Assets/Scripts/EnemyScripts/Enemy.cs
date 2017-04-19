using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public GameObject player;

	public EnemyWeapon weapon;
	public float bulletSpeed;
	public float bulletSize;
	public float bulletDelay;
	public float bulletDamage;
	public float contactDamage;

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

	//some useful methods for enemy AIs
	public bool lookingAtPlayer(){
		Ray forwards = new Ray (transform.position, transform.forward);
		RaycastHit[] hits = Physics.RaycastAll (forwards, Vector3.Distance(transform.position, player.transform.position));
		bool playerHit = false;
		foreach(RaycastHit h in hits){
			if (h.collider.gameObject.name.Equals ("PlayerCollider")) {
				playerHit = true;
			} else if (h.collider.tag == "Wall") {
				return false;
			}
		}

		return playerHit;
	}

	public bool canSeePlayer(){
		Ray forwards = new Ray (transform.position, player.transform.position);
		RaycastHit[] hits = Physics.RaycastAll (forwards, Vector3.Distance(transform.position, player.transform.position));
		foreach(RaycastHit h in hits){
			if (h.collider.gameObject.name.Equals("PlayerCollider")) 
				return true;
		}
		return false;
	}

	public float getContactDamage(){
		return contactDamage;
	}
}
