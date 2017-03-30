﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dual : Weapon {

	void Start () {
		weaponName = "dual";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public override float getBulletDelay(float bulletDelay){
		return bulletDelay*1.5f;
	}

	public override void fire(float bulletSpeed, float bulletDamage, float bulletSize, Vector3 position){
		Vector3 direction = base.getDirection ();
		for (float i = -0.5f; i <= 0.5f; i = i +1) { 
			if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow)) {
				position.x = position.x + i;
			} else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.LeftArrow)) {
				position.z = position.z + i;
			}
			GameObject bul = (GameObject)Instantiate (bullet, position + direction, Quaternion.identity); //transform.position + direction so the bullet starts in front of the player
			bul.transform.localScale = bul.transform.localScale * bulletSize; //scale the size of the bullet.  The bullet's collider should also scale with this
			bul.GetComponent<BulletScript> ().setDamage (bulletDamage);
			bul.GetComponent<BulletScript> ().setSpeed (bulletSpeed);
			Rigidbody rb = bul.GetComponent<Rigidbody> ();
			rb.AddForce (direction * bulletSpeed);

		}
	}
}
