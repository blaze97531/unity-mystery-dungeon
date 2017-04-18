using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour {
	public float movementSpeed = 7.5f;
	public float bulletSpeed = 500.0f;
	public float bulletKnockBack= 1;
	public float bulletSize = 1f; //possibly scale this with damage
	public float bulletDelay = 0.5f; //at 0, the gun fires every frame
	public float bulletDamage = 5f;
	public float invincibilityTime = 0.5f;
	private float invincibleTime;
	public Weapon weapon;
	private float currTime = 0;

	public float maxHealth = 10;
	public float currentHealth;

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W)) {
			transform.Translate (Time.deltaTime * movementSpeed * Vector3.forward);
		}
		if (Input.GetKey (KeyCode.S)) {
			transform.Translate (Time.deltaTime * movementSpeed * Vector3.back);
		}
		if (Input.GetKey (KeyCode.A)) {
			transform.Translate (Time.deltaTime * movementSpeed * Vector3.left);
		}
		if (Input.GetKey (KeyCode.D)) {
			transform.Translate (Time.deltaTime * movementSpeed * Vector3.right);
		}
	}
	void FixedUpdate () {
		MeshRenderer mesh = transform.FindChild ("PlayerModel").GetComponent<MeshRenderer> ();
		if (Time.time < invincibleTime) {
			if (mesh.enabled) {
				mesh.enabled = false;
			} else {
				mesh.enabled = true;
			}
		} else if( Time.time >= invincibleTime && !mesh.enabled){
			mesh.enabled = true;
		}
		currTime = currTime + Time.fixedDeltaTime;
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.RightArrow)||Input.GetKey (KeyCode.LeftArrow)) {
			if (currTime >= (weapon.getBulletDelay(bulletDelay))) { //my current formula for bullet delay
				weapon.fire (bulletSpeed, bulletDamage, bulletSize, bulletKnockBack, transform.position);
				currTime = 0;
			}
		}
	}
	public void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("EnemyBullet")) {
			float damageInflicted = other.GetComponent<EnemyBulletScript> ().getDamage();
			Destroy (other.gameObject);
			if(Time.time >= invincibleTime) //player gets invicibility for a short time when he gets hit
				inflictDamage (damageInflicted);
		}
		if (other.CompareTag ("Enemy")) {
			float damageInflicted = other.GetComponent<Enemy> ().getContactDamage();
			if(Time.time >= invincibleTime) 
				inflictDamage (damageInflicted);
		}
	}

	public void inflictDamage (float amount) {
		currentHealth -= amount;
		invincibleTime = Time.time + invincibilityTime;
		if (currentHealth <= 0.0f) {
			//Destroy (gameObject); Player lost
		}
	}
}