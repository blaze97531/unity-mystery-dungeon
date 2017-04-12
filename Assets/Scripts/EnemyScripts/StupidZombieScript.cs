using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidZombieScript : Enemy {
	
	// Update is called once per frame
	void Update () {
		if(Time.time < knockBackDuration){
			transform.Translate (knockBackDirection *10* Time.deltaTime);
		}
		else{
			Vector3 dir = player.transform.position - transform.position;
			dir.y = 0;
			Quaternion rot = Quaternion.LookRotation (dir);
			transform.rotation = Quaternion.Slerp(transform.rotation, rot, turnSpeed * Time.deltaTime);
			transform.Translate (Time.deltaTime * movementSpeed * Vector3.forward);
		}
	}



}
