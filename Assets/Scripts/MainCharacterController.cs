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
	private Text roomsClearedUI;
	private Text enemiesKilledUI;
	private Text bossName;
	private Slider bossHealthBar;
	private GameObject bossCanvas;

	public int numRoomsCleared;
	public int numEnemiesKilled;

	private GameObject itemPickupCanvasPrefab;
	private GameObject teleportToBossCanvasPrefab;
	private GameObject gameCompleteCanvasPrefab;
	private Dictionary<Collider, GameObject> itemInfoCanvases;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		currentHealth = maxHealth;

		itemPickupCanvasPrefab = Resources.Load<GameObject> ("Prefab/ItemPickupCanvas");
		teleportToBossCanvasPrefab = Resources.Load<GameObject> ("Prefab/BossTeleportPrompt");
		gameCompleteCanvasPrefab = Resources.Load<GameObject> ("Prefab/GameCompleteUI");

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
		roomsClearedUI = GameObject.Find ("RoomsCleared").GetComponent<Text>();
		enemiesKilledUI = GameObject.Find ("EnemiesKilled").GetComponent<Text> ();
		bossCanvas = GameObject.Find ("BossUI");
		bossHealthBar = GameObject.Find ("BossHealthBar").GetComponent<Slider> ();
		bossName = GameObject.Find ("BossName").GetComponent<Text> ();
		bossCanvas.SetActive (false);

		UpdateHealthUI ();
		UpdateWeaponAndStatsUI ();
		UpdateScoreUI ();

		itemInfoCanvases = new Dictionary<Collider, GameObject> ();
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
	public void OnTriggerStay (Collider other) {
		if (other.CompareTag ("Weapon")) {
			if (Input.GetKey (KeyCode.E)) {
				weapon = other.GetComponent<Weapon> ();
				Destroy (other.gameObject);
				UpdateWeaponAndStatsUI ();
				DestroyItemInfoCanvas (other);
			}
		}
		if (other.CompareTag ("Item")) {
			if (Input.GetKey (KeyCode.E)) {
				other.GetComponent<Item> ().apply (this);
				Destroy (other.gameObject);
				UpdateWeaponAndStatsUI ();
				UpdateHealthUI ();
				DestroyItemInfoCanvas (other);
			}
		}
		if (other.CompareTag ("BossTeleporter") && Input.GetKey (KeyCode.E)) {
			// Teleport player to boss.
			transform.position = new Vector3 (-250.0f, transform.position.y, -257.5f);
			DestroyItemInfoCanvas (other);

			GameObject.FindWithTag("Root").GetComponent<GenerateRoom>().SpawnABoss();
		}
	}
		
	public void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("EnemyBullet")) {
			float damageInflicted = other.GetComponent<EnemyBulletScript> ().getDamage();
			Destroy (other.gameObject);
			if(Time.time >= invincibleTime) //player gets invicibility for a short time when he gets hit
				inflictDamage (damageInflicted);
		}
		if (other.CompareTag ("Bandage") && currentHealth < maxHealth) {
			if (currentHealth + 1 > maxHealth)
				currentHealth = maxHealth;
			else
				currentHealth = currentHealth + 1;
			Destroy (other.gameObject);
			UpdateHealthUI ();
		}
		if (other.CompareTag ("Item")) {
			GameObject canvas = Instantiate<GameObject> (itemPickupCanvasPrefab, other.transform.position + 3.5f * Vector3.up, Quaternion.Euler (90.0f, 0.0f, 0.0f));
			FillOutItemInfoCanvas (canvas, null, other.GetComponent<Item> ());
			itemInfoCanvases.Add(other, canvas);
		}
		if (other.CompareTag ("Weapon")) {
			GameObject canvas = Instantiate<GameObject> (itemPickupCanvasPrefab, other.transform.position + 3.5f * Vector3.up, Quaternion.Euler (90.0f, 0.0f, 0.0f));
			FillOutItemInfoCanvas(canvas, (other.gameObject.GetComponent<Weapon>()), null);
			itemInfoCanvases.Add(other, canvas);
		}
		if (other.CompareTag ("BossTeleporter")) {
			GameObject canvas = Instantiate<GameObject> (teleportToBossCanvasPrefab, other.transform.position + 3.5f * Vector3.up, Quaternion.Euler (90.0f, 0.0f, 0.0f));
			itemInfoCanvases.Add (other, canvas);
		}
	}

	public void OnTriggerExit (Collider other) {
		DestroyItemInfoCanvas (other);
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
		invincibleTime = Time.time + invincibilityTime;
		if (currentHealth <= 0.0f) {
			// Need to show death screen.
			ShowGameCompleteUI("You died.");
			Destroy (gameObject); 
		}
	}

	private void UpdateHealthUI () {
		healthBar.maxValue = maxHealth;
		healthBar.value = currentHealth;
		healthText.text = currentHealth.ToString("F2") + " / " + maxHealth.ToString("F2");
	}

	private void UpdateWeaponAndStatsUI () {
		float damage = bulletDamage;
		float delay = bulletDelay;
		float speed = bulletSpeed;
		float size = bulletSize;
		float knockback = bulletKnockBack;

		string delayM = "", speedM = "", damageM = "", sizeM = "", knockbackM = "";

		weapon.ApplyMultipliers (ref delay, ref speed, ref damage, ref size, ref knockback);
		weapon.GetMultiplierStrings (out delayM, out speedM, out damageM, out sizeM, out knockbackM);

		movementSpeedText.text = movementSpeed.ToString("F2");
		bulletDamageText.text = GetStatString (weapon.bulletDamageMultiplier, bulletDamage, damage, damageM);
		bulletDelayText.text = GetStatString (weapon.bulletDelayMultiplier, bulletDelay, delay, delayM);
		bulletSpeedText.text = GetStatString (weapon.bulletSpeedMultiplier, bulletSpeed, speed, speedM);
		bulletSizeText.text = GetStatString (weapon.bulletSizeMultiplier, bulletSize, size, sizeM);
		bulletKnockbackText.text = GetStatString(weapon.bulletKnockbackMultiplier, bulletKnockBack, knockback, knockbackM);

		weaponText.text = weapon.weaponName;
		weaponCooldownBar.maxValue = delay;
		UpdateWeaponCooldownUI ();
	}

	private static string GetStatString (float weaponMultiplierNum, float baseStat, float modStat, string multiplierString) {
		if (weaponMultiplierNum == 1) {
			return baseStat.ToString ("F2");
		} else {
			return modStat.ToString ("F2") + " (" + baseStat.ToString("F2") + multiplierString + ")";
		}
	}

	private void UpdateWeaponCooldownUI () {
		weaponCooldownBar.value = weapon.currTime;
	}

	public void UpdateScoreUI () {
		roomsClearedUI.text = numRoomsCleared.ToString ();
		enemiesKilledUI.text = numEnemiesKilled.ToString ();
	}

	public void EnableBossUI (float currentHealth, float maxHealth, string name) {
		bossHealthBar.maxValue = maxHealth;
		bossName.text = name;
		UpdateBossUI (currentHealth, maxHealth);
		bossCanvas.SetActive (true);
	}

	public void UpdateBossUI (float currentHealth, float maxHealth) {
		bossHealthBar.value = currentHealth;
	}

	public void DisableBossUI () {
		bossCanvas.SetActive (false);
	}

	public void ShowGameCompleteUI (string message) {
		Instantiate<GameObject> (gameCompleteCanvasPrefab).GetComponent<GameCompleteUIScript>().SetMessage(message);
	}

	public void IncEnemiesKilled () {
		numEnemiesKilled++;
		UpdateScoreUI ();
	}

	public void IncRoomsCleared () {
		numRoomsCleared++;
		UpdateScoreUI ();
	}

	/* Usage convention of this function: either w or i should be null. */
	private void FillOutItemInfoCanvas (GameObject canvas, Weapon w, Item i) {
		RectTransform canvasTransform = canvas.GetComponent<RectTransform> ();
		Text itemName = canvasTransform.Find ("ItemNameText").GetComponent<Text> ();
		Text itemDescriptionText = canvasTransform.Find ("ItemDescriptionText").GetComponent<Text> ();

		RectTransform statUi = canvasTransform.Find ("StatUpdateUI").GetComponent<RectTransform> ();
		Text moveSpeed = statUi.Find ("MovementSpeed").GetComponent<Text> ();
		Text bulletDamage = statUi.Find ("BulletDamage").GetComponent<Text> ();
		Text bulletDelay = statUi.Find ("BulletDelay").GetComponent<Text> ();
		Text bulletSpeed = statUi.Find ("BulletSpeed").GetComponent<Text> ();
		Text bulletSize = statUi.Find ("BulletSize").GetComponent<Text> ();
		Text bulletKnockback = statUi.Find ("BulletKnockback").GetComponent<Text> ();
		Text maxHealth = statUi.Find ("MaxHealth").GetComponent<Text> ();

		string mSpeed = "", damage = "", delay = "", speed = "", size = "", knockback = "", health = "";

		if (w != null) {
			const string NO_EFFECT = " 0 ";
			mSpeed = NO_EFFECT;
			health = NO_EFFECT;
			w.GetMultiplierStrings (out delay, out speed, out damage, out size, out knockback);

			itemName.text = w.weaponName;
			itemDescriptionText.text = w.weaponDescription;
		} else if (i != null) {
			itemName.text = i.itemName;
			itemDescriptionText.text = i.itemDescription;

			i.GetModifierStrings (out mSpeed, out damage, out delay, out speed, out size, out knockback, out health);
		} else {
			throw new UnityException ("Illegal call to FillOutItemInfoCanvas: both w & i null");
		}

		moveSpeed.text = mSpeed;
		bulletDamage.text = damage;
		bulletDelay.text = delay;
		bulletSpeed.text = speed;
		bulletSize.text = size;
		bulletKnockback.text = knockback;
		maxHealth.text = health;
	}

	private void DestroyItemInfoCanvas (Collider other) {
		GameObject itemInfoCanvas;
		itemInfoCanvases.TryGetValue (other, out itemInfoCanvas);
		if (itemInfoCanvas != null) {
			Destroy (itemInfoCanvas);
			itemInfoCanvases.Remove (other);
		}
	}


}