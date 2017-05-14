using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sniper : Weapon {

	void Start () {
		bullet = Resources.Load("Prefab/Bullet");
	}
		

	public override void fire(Vector3 position, Vector3 velocity, Vector3 target, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack){
		base.fire (position, velocity, target, bulletDelay*3, bulletSpeed * 2, bulletDamage*5, bulletSize,bulletKnockBack*2);
	}
}
