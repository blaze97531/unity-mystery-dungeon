using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sniper : Weapon {

	void Start () {
		weaponName = "sniper";
		bullet = Resources.Load("Prefab/Bullet");
	}
		
	public override void fire(Vector3 velocity, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack, Vector3 position){
		base.fire (velocity, bulletDelay*3, bulletSpeed * 2, bulletDamage*5, bulletSize,bulletKnockBack*2, position);
	}
}
