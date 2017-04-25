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
	public float invincibilityTime = 1;
	public Vector3 velocity;

	private float invincibleTime;
	public Weapon weapon;

	public float maxHealth = 10;
	public float currentHealth;


	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
		velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W)) {
			velocity = Vector3.ClampMagnitude (velocity + Vector3.forward * Time.deltaTime * 60, movementSpeed);
		} 
		if (Input.GetKey (KeyCode.S)) {
			velocity = Vector3.ClampMagnitude (velocity + Vector3.back * Time.deltaTime * 60, movementSpeed);
		} 
		if (Input.GetKey (KeyCode.A)) {
			velocity = Vector3.ClampMagnitude (velocity + Vector3.left * Time.deltaTime * 60, movementSpeed);
		} 
		if (Input.GetKey (KeyCode.D)) {
			velocity = Vector3.ClampMagnitude (velocity + Vector3.right * Time.deltaTime * 60, movementSpeed);
		} 
		if (!Input.GetKey (KeyCode.W) && !Input.GetKey (KeyCode.S) && !Input.GetKey (KeyCode.A) && !Input.GetKey (KeyCode.D)) {
			velocity = velocity * Time.deltaTime * 50;
		}
		transform.Translate (transform.worldToLocalMatrix * (Time.deltaTime * velocity));
		weapon.fire ((velocity.magnitude/movementSpeed * 0.3f) *velocity.normalized, bulletDelay, bulletSpeed, bulletDamage, bulletSize, bulletKnockBack, transform.position);
	}

	void FixedUpdate(){
		MeshRenderer mesh = transform.GetComponent<MeshRenderer> ();
		if (Time.time < invincibleTime) {
			if (Time.time % 0.25 < 0.125) {
				mesh.enabled = false;
			} else if (Time.time % 0.25 >= 0.125) {
				mesh.enabled = true;
			}
		} else if( Time.time >= invincibleTime && !mesh.enabled){
			mesh.enabled = true;
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
		invincibleTime = Time.time + (invincibilityTime * amount);
		if (currentHealth <= 0.0f) {
			//Destroy (gameObject); Player lost
		}
	}
}