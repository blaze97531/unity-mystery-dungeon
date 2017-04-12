using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy {
	public bool canSee;

	// Update is called once per frame
	void Update () {
		Vector3 dir = player.transform.position - transform.position;
		dir.y = 0;
		Quaternion rot = Quaternion.LookRotation (dir);
		transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
		canSee = canSeePlayer ();
			currTime = currTime + Time.deltaTime;
		if (canSeePlayer ()) {
			if (currTime >= (weapon.getBulletDelay(bulletDelay))) {
				weapon.fire (bulletSpeed, bulletDamage, bulletSize, bulletKnockBack, transform.position);
				currTime = 0;
			}
		}
	}

	bool canSeePlayer(){
		Ray forwards = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (forwards, out hit)) {
			if (hit.collider != null) {
				return hit.collider.gameObject.name.Equals("PlayerCollider");
			}
		}
		return false;
	}
}
