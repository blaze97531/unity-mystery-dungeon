﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shotgun : Weapon {

	void Start () {
		weaponName = "shotgun";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public override void fire(Vector3 velocity, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack, Vector3 position){
		bulletDelay = bulletDelay * 3;
		Vector3 direction = getDirection () + velocity;
		currTime = currTime + Time.deltaTime;
		if (currTime >= bulletDelay	&& (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.LeftArrow))) {
			for (int i = -2; i <= 2; i++) { //essentially fires 5 bullets in an arch
				GameObject bul = (GameObject)Instantiate (bullet, position + getGunLocationOffset (), Quaternion.identity);
				bul.transform.localScale = bul.transform.localScale * bulletSize;
				bul.GetComponent<BulletScript> ().setDamage (bulletDamage);
				bul.GetComponent<BulletScript> ().setKnockBack (bulletKnockBack);
				bul.GetComponent<BulletScript> ().setDirection (direction);
				Rigidbody rb = bul.GetComponent<Rigidbody> ();
				if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow)) {	//I cant say this math entirely checks out, but it actually looks really nice in game
					rb.AddForce (new Vector3 (direction.x + Mathf.Sin (i * Mathf.PI / 28), 0, direction.z * Mathf.Cos (i * Mathf.PI / 12)) * bulletSpeed);
				} else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.LeftArrow)) {
					rb.AddForce (new Vector3 (direction.x * Mathf.Cos (i * Mathf.PI / 12), 0, direction.z + Mathf.Sin (i * Mathf.PI / 28)) * bulletSpeed);
				}
			}
			currTime = 0;
		}
	}
}
