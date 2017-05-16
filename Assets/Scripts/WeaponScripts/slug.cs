using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slug : Weapon {

	public override void fire(Vector3 position, Vector3 velocity, Vector3 target, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack){
		base.fire (position, velocity, target, bulletDelay*3, bulletSpeed/1.25f, bulletDamage*5, bulletSize*3, bulletKnockBack*2);
	}
}
