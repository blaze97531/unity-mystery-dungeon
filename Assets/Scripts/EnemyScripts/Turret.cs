﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy {
	
	// Update is called once per frame
	void Update () {
		Vector3 dir = player.transform.position - transform.position;
		dir.y = 0;
		Quaternion rot = Quaternion.LookRotation (dir,Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
		currTime = currTime + Time.deltaTime;
		if (lookingAtPlayer ()) {
			if (currTime >= (weapon.getBulletDelay(bulletDelay))) {
				weapon.fire (bulletSpeed, bulletDamage, bulletSize, transform.position, transform.forward);
				currTime = 0;
			}
		}
	}
}
