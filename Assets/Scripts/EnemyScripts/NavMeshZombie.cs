﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshZombie : Enemy {
	NavMeshAgent agent;
	float time;
	new void Start(){
		agent = GetComponent<NavMeshAgent> ();
		time = 0;
		base.Start ();
	}
	// Update is called once per frame
	void FixedUpdate () {
		agent.SetDestination (player.GetComponent<Transform> ().position);

		Vector3 dir = agent.desiredVelocity;
		dir.y = 0;
		Quaternion rot = Quaternion.LookRotation (dir,Vector3.up);
		transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, rot,turnSpeed * Time.deltaTime);
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
