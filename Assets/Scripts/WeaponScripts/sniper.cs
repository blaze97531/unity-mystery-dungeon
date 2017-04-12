using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sniper : Weapon {

	void Start () {
		weaponName = "sniper";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public override float getBulletDelay(float bulletDelay){
		return bulletDelay*3;
	}

	public override void fire(float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack, Vector3 position){
		base.fire (bulletSpeed * 2, bulletDamage*5, bulletSize,bulletKnockBack*2, position);
	}
}
