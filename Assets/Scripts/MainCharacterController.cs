using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	private Slider healthBar;
	private Text healthText;
	private Text movementSpeedText;
	private Text bulletDamageText;
	private Text bulletDelayText;
	private Text bulletSpeedText;
	private Text bulletSizeText;
	private Text bulletKnockbackText;
	private Text weaponText;
	private Slider weaponCooldownBar;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		currentHealth = maxHealth;

		healthBar = (Slider)GameObject.Find ("HealthBar").GetComponent<Slider> ();
		healthText = (Text)GameObject.Find ("HealthText").GetComponent<Text> ();
		movementSpeedText = (Text)GameObject.Find ("MovementSpeed").GetComponent<Text> ();
		bulletDamageText = (Text)GameObject.Find ("BulletDamage").GetComponent<Text> ();
		bulletDelayText = (Text)GameObject.Find ("BulletDelay").GetComponent<Text> ();
		bulletSpeedText = (Text)GameObject.Find ("BulletSpeed").GetComponent<Text> ();
		bulletSizeText = (Text)GameObject.Find ("BulletSize").GetComponent<Text> ();
		bulletKnockbackText = (Text)GameObject.Find ("BulletKnockback").GetComponent<Text> ();
		weaponText = (Text)GameObject.Find ("WeaponText").GetComponent<Text> ();
		weaponCooldownBar = (Slider)GameObject.Find ("WeaponCooldownBar").GetComponent<Slider> ();

		UpdateHealthUI ();
		UpdateWeaponAndStatsUI ();
	}

	void Update () {
		weapon.Cooldown ();
		UpdateWeaponCooldownUI ();
	}

	private void ShootAtAndRotateTowards (Vector3 target) {
		Vector3 direction = target - transform.position;

		rb.MoveRotation(Quaternion.Euler (Quaternion.LookRotation (direction).eulerAngles + new Vector3(-90.0f, 180.0f, 0.0f)));

		weapon.fire (transform.position, velocity, target, bulletDelay, bulletSpeed, bulletDamage, bulletSize, bulletKnockBack);
	}

	void FixedUpdate(){
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
			// TODO JRH: I think this line could cause some bad behavior, particularly if there is lag.
			// If Time.deltaTime is greater than 0.02 seconds, then, 50 * Time.deltaTime will be greater
			// than 1, and the velocity will INCREASE, not decrease.
			velocity = velocity * Time.fixedDeltaTime * 45;
		}
		rb.MovePosition(transform.position + Time.deltaTime * velocity);

		// Left Mouse
		if (Input.GetMouseButton(0)) {
			RaycastHit info;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info, float.PositiveInfinity)) {
				ShootAtAndRotateTowards(new Vector3 (info.point.x, transform.position.y, info.point.z));
			}

		} else {
			// Shooting with keyboard is still an option (because I don't want to shoot with a track pad.
			if (Input.GetKey(KeyCode.UpArrow)) {
				ShootAtAndRotateTowards(transform.position + Vector3.forward);
			} else if (Input.GetKey(KeyCode.DownArrow)) {
				ShootAtAndRotateTowards(transform.position + Vector3.back);
			} else if (Input.GetKey(KeyCode.LeftArrow)) {
				ShootAtAndRotateTowards(transform.position + Vector3.left);
			} else if (Input.GetKey(KeyCode.RightArrow)) {
				ShootAtAndRotateTowards(transform.position + Vector3.right);
			} else {
				/* Handle rotation if the player is not shooting */
				if (Input.GetKey (KeyCode.W)) {
					transform.eulerAngles = new Vector3 (270, 180, 0);
				} else if (Input.GetKey (KeyCode.S)) {
					transform.eulerAngles = new Vector3 (270, 0, 0);
				} else if (Input.GetKey (KeyCode.A)) {
					transform.eulerAngles = new Vector3 (270, 90, 0);
				} else if (Input.GetKey (KeyCode.D)) {
					transform.eulerAngles = new Vector3 (270, 270, 0);
				}
			}
		}


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
		if (other.CompareTag ("Weapon")) {
			weapon = other.GetComponent<Weapon>();
			Destroy(other.gameObject);
			UpdateWeaponAndStatsUI ();
		}
		if (other.CompareTag ("Item")) {
			other.GetComponent<Item>().apply(this);
			Destroy(other.gameObject);
			UpdateWeaponAndStatsUI ();
			UpdateHealthUI ();
		}
		if (other.CompareTag ("Bandage") && currentHealth < maxHealth) {
			if (currentHealth + 1 > maxHealth)
				currentHealth = maxHealth;
			else
				currentHealth = currentHealth + 1;
			Destroy (other.gameObject);
			UpdateHealthUI ();
		}
	}
	public void OnCollisionStay (Collision other) {
		if (other.collider.CompareTag ("Enemy")) {
			float damageInflicted = other.collider.GetComponent<Enemy> ().getContactDamage();
			if(Time.time >= invincibleTime) 
				inflictDamage (damageInflicted);
		}
	}

	public void inflictDamage (float amount) {
		currentHealth -= amount;
		UpdateHealthUI ();
		invincibleTime = Time.time + (invincibilityTime * amount);
		if (currentHealth <= 0.0f) {
			//Destroy (gameObject); Player lost
		}
	}

	private void UpdateHealthUI () {
		healthBar.maxValue = maxHealth;
		healthBar.value = currentHealth;
		healthText.text = currentHealth + " / " + maxHealth;
	}

	private void UpdateWeaponAndStatsUI () {
		float damage = bulletDamage;
		float delay = bulletDelay;
		float speed = bulletSpeed;
		float size = bulletSize;
		float knockback = bulletKnockBack;

		weapon.ApplyMultipliers (ref delay, ref speed, ref damage, ref size, ref knockback);

		movementSpeedText.text = movementSpeed.ToString();
		bulletDamageText.text = damage.ToString ();
		bulletDelayText.text = delay.ToString();
		bulletSpeedText.text = speed.ToString();
		bulletSizeText.text = size.ToString();
		bulletKnockbackText.text = knockback.ToString();

		weaponText.text = weapon.weaponName;
		weaponCooldownBar.maxValue = delay;
		UpdateWeaponCooldownUI ();
	}

	private void UpdateWeaponCooldownUI () {
		weaponCooldownBar.value = weapon.currTime;
	}
}