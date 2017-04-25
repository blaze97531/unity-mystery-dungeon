﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dual : Weapon {

	void Start () {
		weaponName = "dual";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public override void fire(Vector3 velocity, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack, Vector3 position){
		bulletDelay = bulletDelay * 1.5f;
		currTime = currTime + Time.fixedDeltaTime;
		if (currTime >= bulletDelay	&& (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.LeftArrow))) {
			Vector3 direction =  getDirection();
			Vector3 pos = position;
			for (float i = -0.25f; i <= 0.25f; i = i + 0.5f) { 
				if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow)) {
					pos.x = position.x + i;
				} else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.LeftArrow)) {
					pos.z = position.z + i;
				}
				GameObject bul = (GameObject)Instantiate (bullet, pos + getGunLocationOffset (), Quaternion.identity); //transform.position + direction so the bullet starts in front of the player
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
