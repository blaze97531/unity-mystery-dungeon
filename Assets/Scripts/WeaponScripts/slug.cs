using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slug : Weapon {

	void Start () {
		weaponName = "slug";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public override float getBulletDelay(float bulletDelay){
		return bulletDelay*3;
	}

	public override void fire(float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack, Vector3 position){
		base.fire (bulletSpeed/1.25f, bulletDamage*5, bulletSize*3, bulletKnockBack*2, position);
	}
}
