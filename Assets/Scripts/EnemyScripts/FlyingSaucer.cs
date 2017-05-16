using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingSaucer : Enemy {
	NavMeshAgent agent;
	public EnemyWeapon weapon2;
	public EnemyWeapon weapon3;
	public string superState;
	public string state;

	float stateTime;
	public Vector3 finalPosition;
	float currTime = 0;
	int chargeNum;
	int numCharges;

	Vector3 center = new Vector3(-250,0.5f,-250);
	new void Start(){
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		stateTime = 0;
		base.Start ();
		superState = "alldirections";
	}

	// Update is called once per frame
	void Update () {
		currTime = currTime + Time.deltaTime;
		if (superState.Equals("alldirections")) {
			if ((transform.position - center).magnitude > 1) {
				agent.SetDestination (center);
				faceDesiredVelocity ();
			} else {
				if (currTime >= bulletDelay) {
					weapon2.fire (bulletDelay, bulletSpeed, bulletDamage, bulletSize, transform.position, Vector3.Normalize (vectorToPlayer ()));
					currTime = 0;
				}
				facePlayer ();
			}
			if (stateTime < Time.time) {
				chooseState (-1);
			}
		}
		if (superState.Equals ("charge")) {
			if (state.Equals ("charging") && (transform.position - finalPosition).magnitude < 1) {
				if (chargeNum >= numCharges) {
					chooseState (-1);
				} else {
					state = "charge";
				}
			} else if (chargeNum < numCharges && state.Equals ("charge")) {
				chargeNum++;

				Vector3 direction = vectorToPlayer()*1.5f;
				direction += transform.position;
				NavMeshHit hit;
				NavMesh.SamplePosition (direction, out hit, direction.magnitude, 1);
				finalPosition = hit.position;

				agent.SetDestination (finalPosition);
				state = "charging";
			}
			faceDesiredVelocity ();
		}
		if (superState.Equals ("trishot")) {
			if (!canSeePlayer ()) {
				chooseState (-1);
			}

			facePlayer ();
			if (currTime >= bulletDelay/2) {
				weapon3.fire (bulletDelay, bulletSpeed+200, bulletDamage, bulletSize+1, transform.position, Vector3.Normalize (vectorToPlayer ()));
				currTime = 0;
			}
			if (stateTime < Time.time) {
				chooseState (-1);
			}
		}
		if (superState.Equals ("singleshot")) {
			if (!canSeePlayer ()) {
				chooseState (-1);
			}

			facePlayer ();
			if (currTime >= bulletDelay) {
				weapon.fire (bulletDelay, bulletSpeed-200, bulletDamage+1, bulletSize+4, transform.position, Vector3.Normalize (vectorToPlayer ()));
				currTime = 0;
			}
			if (stateTime < Time.time) {
				chooseState (-1);
			}
		}
	}

	new void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("Bullet")) {
			float damageInflicted = other.GetComponent<BulletScript> ().getDamage();
			Destroy (other.gameObject);
			inflictDamage (damageInflicted);
		}
	}

	void chooseState(int rnd){
		if (rnd < 0)
			rnd = Random.Range (0, 4);
		if (rnd == 0) {
			superState = "alldirections";
			int length = Random.Range (2, 5);
			stateTime = Time.time + length;
			Debug.Log ("alldirections " + length);
		} else if (rnd == 1) {
			superState = "charge";
			state = "charge";
			chargeNum = 0;
			numCharges = Random.Range (1, 4);
			Debug.Log ("charge " + numCharges);
		} else if (rnd == 2) {
			superState = "trishot";
			int length = Random.Range (1, 3);
			stateTime = Time.time + length;
			Debug.Log ("trishot " + length);
		} else if (rnd == 3) {
			superState = "singleshot";
			int length = Random.Range (1, 2);
			stateTime = Time.time + length;
			Debug.Log ("singleshot " + length);
		}
	}

	void facePlayer(){
		Vector3 faceDir = vectorToPlayer ();
		faceDir.y = 0;
		if (!faceDir.Equals (Vector3.zero)) {
			Quaternion rot = Quaternion.LookRotation (faceDir, Vector3.up);
			rb.rotation = Quaternion.SlerpUnclamped (rb.rotation, rot, turnSpeed * Time.deltaTime);
		}
	}

	void faceDesiredVelocity(){
		Vector3 dir = agent.desiredVelocity;
		dir.y = 0;
		if (!dir.Equals (Vector3.zero)) {
			Quaternion rot = Quaternion.LookRotation (dir, Vector3.up);
			rb.rotation = Quaternion.SlerpUnclamped (rb.rotation, rot, turnSpeed * Time.deltaTime);
		}
	}
}
