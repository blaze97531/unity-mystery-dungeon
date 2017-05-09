using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy {
	float currTime = 0;
	// Update is called once per frame
	void Update () {
		if (canSeePlayer ()) {
			Vector3 dir = vectorToPlayer ();
			dir.y = 0;
			Quaternion rot = Quaternion.LookRotation (dir, Vector3.up);
			transform.rotation = Quaternion.SlerpUnclamped (transform.rotation, rot, turnSpeed * Time.deltaTime);
			currTime = currTime + Time.deltaTime;
			if (currTime >= bulletDelay) {
				if (lookingAtPlayer ()) {
					weapon.fire (bulletDelay, bulletSpeed, bulletDamage, bulletSize, transform.position, transform.forward);
					currTime = 0;
				}
			}
		}
	}
}
