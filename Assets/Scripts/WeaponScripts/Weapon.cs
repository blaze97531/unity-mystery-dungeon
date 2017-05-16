using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
	public string weaponName;
	public Object bullet;

	public float currTime = 0;

	/* For these multipliers: Positive indicates favorable, negative indicates unfavorable. Absolute value should be >= 1. */
	public float bulletDelayMultiplier;
	public float bulletSpeedMultiplier;
	public float bulletDamageMultiplier;
	public float bulletSizeMultiplier;
	public float bulletKnockbackMultiplier;

	void Start () {
		bullet = Resources.Load("Prefab/Weapons/Bullet");
	}

	void Update() {
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
	}

	public void Cooldown () {
		currTime = currTime + Time.deltaTime;
	}

	public virtual float getBulletDelay(float bulletDelay){
		return bulletDelay;
	}

	public void ApplyMultipliers (ref float bulletDelay, ref float bulletSpeed, ref float bulletDamage, ref float bulletSize, ref float bulletKnockback) {
		if (bulletDelayMultiplier < 0) {
			bulletDelay *= -bulletDelayMultiplier;
		} else {
			bulletDelay /= bulletDelayMultiplier;
		}

		if (bulletDamageMultiplier < 0) {
			bulletDamage /= -bulletDamageMultiplier;
		} else {
			bulletDamage *= bulletDamageMultiplier;
		}

		if (bulletSizeMultiplier < 0) {
			bulletSize /= -bulletSizeMultiplier;
		} else {
			bulletSize *= bulletSizeMultiplier;
		}

		if (bulletSpeedMultiplier < 0) {
			bulletSpeed /= -bulletSpeedMultiplier;
		} else {
			bulletSpeed *= bulletSpeedMultiplier;
		}

		if (bulletKnockbackMultiplier < 0) {
			bulletKnockback /= -bulletKnockbackMultiplier;
		} else {
			bulletKnockback *= bulletKnockbackMultiplier;
		}
	}
		
	/* Pass in the player's base stats into this method. */
	public virtual void fire(Vector3 position, Vector3 velocity, Vector3 target, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack) {
		ApplyMultipliers (ref bulletDelay, ref bulletSpeed, ref bulletDamage, ref bulletSize, ref bulletKnockBack);
		if (currTime >= bulletDelay) {
			Vector3 direction = (target - position).normalized;
			Vector3 shotOrigin = position + getGunLocationOffset(direction);

			GameObject bul = (GameObject)Instantiate (bullet, shotOrigin, Quaternion.identity); //transform.position + direction so the bullet starts in front of the player
			bul.transform.localScale = bul.transform.localScale * bulletSize; //scale the size of the bullet.  The bullet's collider should also scale with this
			bul.GetComponent<BulletScript> ().setDamage (bulletDamage);
			bul.GetComponent<BulletScript> ().setKnockBack (bulletKnockBack);
			bul.GetComponent<BulletScript> ().setDirection (direction);
			Rigidbody rb = bul.GetComponent<Rigidbody> ();
			rb.AddForce (direction * bulletSpeed);
			currTime = 0;
		}
	}

	public virtual Vector3 getGunLocationOffset(Vector3 direction){
		Vector3 perpendicularLeft = GetPerpendicularLeft (direction);
		Vector3 offset = 1.0f * direction + perpendicularLeft * 0.35f + 0.65f * Vector3.up;
		return offset;
	}

	/* Returns v rotated 90 degrees left (about the y axis). */
	public Vector3 GetPerpendicularLeft (Vector3 v) {
		return new Vector3(-v.z, v.y, v.x);
	}
}
