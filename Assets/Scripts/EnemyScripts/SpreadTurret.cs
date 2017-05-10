using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadTurret : Enemy {
	float currTime = 0;
	// Update is called once per frame
	void Update () {
		Vector3 dir = vectorToPlayer();
		dir.y = 0;
		Quaternion rot = Quaternion.LookRotation (dir,Vector3.up);
		transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, rot,1);
		currTime = currTime + Time.deltaTime;
		if (currTime >= bulletDelay) {
			if (canSeePlayer ()) {
				weapon.fire (bulletDelay, bulletSpeed, bulletDamage, bulletSize, transform.position, Vector3.Normalize(vectorToPlayer()));
				currTime = 0;
			}
		}
	}
}
