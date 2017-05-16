using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienSoldier : Enemy {
	NavMeshAgent agent;
	public float wanderDistance;
	public string state;
	public bool lookingAtPlayer;

	Vector3 finalPosition;
	float currTime = 0;

	new void Start(){
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		base.Start ();
		state = "patrol";
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		currTime = currTime + Time.fixedDeltaTime;
		lookingAtPlayer = lookingAtPlayer ();
		if (state.Equals ("patrolling") && agent.velocity.Equals (Vector3.zero)) {
			state = "shooting";
		}
		if(state.Equals("shooting")){
			if (canSeePlayer () && currTime >= bulletDelay) {
				Vector3 dir = vectorToPlayer ();
				dir.y = 0;
				Quaternion rot = Quaternion.LookRotation (dir, Vector3.up);
				transform.rotation = Quaternion.SlerpUnclamped (transform.rotation, rot, turnSpeed * Time.fixedDeltaTime);
				if (lookingAtPlayer ()) {
					weapon.fire (bulletDelay, bulletSpeed, bulletDamage, bulletSize, transform.position, Vector3.Normalize (vectorToPlayer ()));
					currTime = 0;
				}
			} else if(!canSeePlayer ())
				state = "patrol";

		}

		if(state.Equals("patrol")){
			Vector3 randomDirection = Random.insideUnitSphere * wanderDistance;

			randomDirection += transform.position;
			NavMeshHit hit;
			NavMesh.SamplePosition (randomDirection, out hit, wanderDistance, 1);
			finalPosition = hit.position;

			agent.SetDestination (finalPosition);
			state = "patrolling";
		}

		if (state.Equals ("patrolling")) {
			Vector3 faceDir = agent.desiredVelocity;
			faceDir.y = 0;
			if (!faceDir.Equals (Vector3.zero)) {
				Quaternion rot = Quaternion.LookRotation (faceDir, Vector3.up);
				rb.rotation = Quaternion.SlerpUnclamped (rb.rotation, rot, turnSpeed * Time.fixedDeltaTime);
			}
		}


	}

	new void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("Bullet")) {
			float damageInflicted = other.GetComponent<BulletScript> ().getDamage();
			float bulletKnockBack = other.GetComponent<BulletScript> ().getKnockBack();
			Vector3 direction = other.GetComponent<BulletScript> ().getDirection();

			Destroy (other.gameObject);
			inflictDamage (damageInflicted);

			direction.y = 0;
			agent.velocity =  agent.velocity + direction * bulletKnockBack/knockBackResistance;
		}
	}
}
