using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smg : Weapon {

	void Start () {
		weaponName = "smg";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public override float getBulletDelay(float bulletDelay){
		return bulletDelay/4;
	}

	public override void fire(float bulletSpeed, float bulletDamage, float bulletSize, Vector3 position){
		base.fire (bulletSpeed, bulletDamage/2.5f, bulletSize/1.5f, position);
	}
}
