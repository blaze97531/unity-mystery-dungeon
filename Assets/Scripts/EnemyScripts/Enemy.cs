using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
	public GameObject player;
	// Note: this field is null unless the enemy is linked to the Boss UI.
	private MainCharacterController playerController;

	public string enemyName;

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

	protected Vector3 forwardsVelocity;
	protected Rigidbody rb;
	protected Vector3 knockBackDirection;

	protected bool linked_to_boss_ui = false;

	protected void Start () {
		rb = GetComponent<Rigidbody> ();
		current_health = MAX_HEALTH;
		player = GameObject.FindWithTag ("Player");
		playerController = player.GetComponent<MainCharacterController> ();
	}

	public virtual void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("Bullet")) {
			float damageInflicted = other.GetComponent<BulletScript> ().getDamage();
			float bulletKnockBack = other.GetComponent<BulletScript> ().getKnockBack();
			Vector3 direction = other.GetComponent<BulletScript> ().getDirection();

			Destroy (other.gameObject);
			inflictDamage (damageInflicted);

			direction.y = 0;
			forwardsVelocity = forwardsVelocity + direction * bulletKnockBack/knockBackResistance;
		}
	}

	public virtual void inflictDamage (float amount) {
		current_health -= amount;
		if (linked_to_boss_ui) {
			playerController.UpdateBossUI (current_health, MAX_HEALTH);
		}

		if (current_health <= 0.0f) {
			playerController.IncEnemiesKilled ();
			if (linked_to_boss_ui) {
				UnlinkFromBossUI ();
			}
			Destroy (gameObject);
		}
	}

	//some useful methods for enemy AIs
	public bool lookingAtPlayer(){
		if (canSeePlayer()){
			Vector3 facing =  Vector3.Normalize(vectorToPlayer());
			facing.y = 0;
			if (Vector3.Angle (transform.forward, facing) < 1)
				return true;
		}
		return false;
	}

	public bool canSeePlayer(){
		bool ret = false;
		Ray direction = new Ray (transform.position, Vector3.Normalize(vectorToPlayer()));
		RaycastHit[] hits = Physics.RaycastAll (direction, Vector3.Distance(transform.position, player.GetComponent<Transform> ().position));
		foreach(RaycastHit h in hits){
			if (h.collider.gameObject.CompareTag("Player")) 
				ret = true;
			if (h.collider.gameObject.tag.Equals("Wall") )
				return false;
		}
		return ret;
	}

	//Returns the vector between the player and the enemy
	public Vector3 vectorToPlayer(){
		Vector3 ret = player.GetComponent<Transform> ().position - transform.position;
		return ret;
	}

	public float getContactDamage(){
		return contactDamage;
	}

	protected void LinkToBossUI () {
		playerController = player.GetComponent<MainCharacterController> ();
		playerController.EnableBossUI (current_health, MAX_HEALTH, enemyName);
		linked_to_boss_ui = true;
	}

	protected void UnlinkFromBossUI () {
		playerController.DisableBossUI ();
		linked_to_boss_ui = false;
	}
}
