using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slug : Weapon {

	void Start () {
		weaponName = "slug";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public override void fire(Vector3 velocity, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack, Vector3 position){
		base.fire (velocity, bulletDelay*3, bulletSpeed/1.25f, bulletDamage*5, bulletSize*3, bulletKnockBack*2, position);
	}
}
