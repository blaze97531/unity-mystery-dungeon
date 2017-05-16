using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dual : Weapon {

	public override void fire(Vector3 position, Vector3 velocity, Vector3 target, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack){
		bulletDelay = bulletDelay * 1.5f;
		if (currTime >= bulletDelay) {
			Vector3 direction =  (target - position).normalized;
			Vector3 pos = position;
			for (float i = -0.25f; i <= 0.25f; i = i + 0.5f) { 
				pos = pos + GetPerpendicularLeft (direction) * i;

				GameObject bul = (GameObject)Instantiate (bullet, pos + getGunLocationOffset (direction), Quaternion.identity); //transform.position + direction so the bullet starts in front of the player
				bul.transform.localScale = bul.transform.localScale * bulletSize; //scale the size of the bullet.  The bullet's collider should also scale with this
				bul.GetComponent<BulletScript> ().setDamage (bulletDamage);
				bul.GetComponent<BulletScript> ().setKnockBack (bulletKnockBack);
				bul.GetComponent<BulletScript> ().setDirection (direction);

				Rigidbody rb = bul.GetComponent<Rigidbody> ();
				rb.AddForce (direction * bulletSpeed);
			}
			currTime = 0;
		}
	}
}
