using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidZombieScript : Enemy {

	// Update is called once per frame
	void FixedUpdate () {
		Vector3 dir = vectorToPlayer();
		dir.y = 0;
		Quaternion rot = Quaternion.LookRotation (dir);
		rb.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.fixedDeltaTime);

		forwardsVelocity = Vector3.ClampMagnitude (forwardsVelocity + transform.forward * 0.7f, movementSpeed); //current bug: this clamps knockback to its max movement speed
		rb.MovePosition(transform.position + Time.fixedDeltaTime * forwardsVelocity);
	}
}
