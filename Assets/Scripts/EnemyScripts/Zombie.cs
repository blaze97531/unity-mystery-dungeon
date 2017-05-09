using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy {

	// Update is called once per frame
	void Update () {
		if (canSeePlayer ()) {
			Vector3 dir = vectorToPlayer ();
			dir.y = 0;
			Quaternion rot = Quaternion.LookRotation (dir);
			rb.rotation = Quaternion.Slerp (transform.rotation, rot, turnSpeed * Time.deltaTime);

			forwardsVelocity = Vector3.ClampMagnitude (forwardsVelocity + transform.forward * 0.7f, movementSpeed); //current bug: this clamps knockback to its max movement speed
			rb.MovePosition (transform.position + Time.deltaTime * forwardsVelocity);
		} else {
			forwardsVelocity = Vector3.ClampMagnitude (forwardsVelocity * 0.95f, movementSpeed); //current bug: this clamps knockback to its max movement speed
			rb.MovePosition (transform.position + Time.deltaTime * forwardsVelocity);
		}
	}
}

