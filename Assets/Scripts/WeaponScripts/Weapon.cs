using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
	public string weaponName;
	public Object bullet;

	protected float currTime = 0;

	void Start () {
		bullet = Resources.Load("Prefab/Bullet");
	}

	public void Cooldown () {
		currTime = currTime + Time.deltaTime;
	}

	public virtual float getBulletDelay(float bulletDelay){
		return bulletDelay;
	}
		
	public virtual void fire(Vector3 position, Vector3 velocity, Vector3 target, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack) {
		if (currTime >= bulletDelay) {
			Vector3 direction = (target - position).normalized;
			Vector3 shotOrigin = position + getGunLocationOffset(direction);

			// Recalculate direction based on gun offset.
			// Maybe not though. This does some strange things if you are clicking directly in front of the player.
			// Also, if shooting with a keyboard, this makes no sense.
			/*
			target.y = shotOrigin.y;
			direction = (target - shotOrigin).normalized; */

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
