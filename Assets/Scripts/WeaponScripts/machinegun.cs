using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class machinegun : Weapon {

	void Start () {
		weaponName = "machinegun";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public override void fire(Vector3 velocity,float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack, Vector3 position){
		base.fire (velocity, bulletDelay/3, bulletSpeed, bulletDamage/1.5f, bulletSize*1.25f, bulletKnockBack/1.5f, position);
	}
}
