using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class machinegun : Weapon {

	void Start () {
		weaponName = "machinegun";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public override float getBulletDelay(float bulletDelay){
		return bulletDelay/3;
	}

	public override void fire(float bulletSpeed, float bulletDamage, float bulletSize, Vector3 position){
		base.fire (bulletSpeed, bulletDamage/1.5f, bulletSize*1.25f, position);
	}
}
