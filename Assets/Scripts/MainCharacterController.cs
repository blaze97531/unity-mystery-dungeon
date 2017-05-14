using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour {
	public float maxHealth = 10;
	public float currentHealth;

	public float movementSpeed = 7.5f;
	public float bulletSpeed = 500.0f;
	public float bulletKnockBack= 1;
	public float bulletSize = 1f; //possibly scale this with damage
	public float bulletDelay = 0.5f; //at 0, the gun fires every frame
	public float bulletDamage = 5f;
	public float invincibilityTime = 1;
	public Weapon weapon;

	public Vector3 velocity;
	public Vector3 closestPoint;

	private float invincibleTime;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
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
		if (Input.GetKey (KeyCode.W)) {
			velocity = Vector3.ClampMagnitude (velocity + Vector3.forward * Time.fixedDeltaTime * 60, movementSpeed);
		} 
		if (Input.GetKey (KeyCode.S)) {
			velocity = Vector3.ClampMagnitude (velocity + Vector3.back * Time.fixedDeltaTime * 60, movementSpeed);
		} 
		if (Input.GetKey (KeyCode.A)) {
			velocity = Vector3.ClampMagnitude (velocity + Vector3.left * Time.fixedDeltaTime * 60, movementSpeed);
		} 
		if (Input.GetKey (KeyCode.D)) {
			velocity = Vector3.ClampMagnitude (velocity + Vector3.right * Time.fixedDeltaTime * 60, movementSpeed);
		} 
		if (!Input.GetKey (KeyCode.W) && !Input.GetKey (KeyCode.S) && !Input.GetKey (KeyCode.A) && !Input.GetKey (KeyCode.D)) {
			velocity = velocity * Time.fixedDeltaTime * 45;
		}
		rb.MovePosition(transform.position + Time.fixedDeltaTime * velocity);

	}

	public void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("EnemyBullet")) {
			float damageInflicted = other.GetComponent<EnemyBulletScript> ().getDamage();
			Destroy (other.gameObject);
			if(Time.time >= invincibleTime) //player gets invicibility for a short time when he gets hit
				inflictDamage (damageInflicted);
		}
	}

	public void OnCollisionStay (Collision other) {
		if (other.collider.CompareTag ("Enemy")) {
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			float damageInflicted = other.collider.GetComponent<Enemy> ().getContactDamage ();
			if (Time.time >= invincibleTime)
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