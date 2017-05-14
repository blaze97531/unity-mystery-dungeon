using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smg : Weapon {

	void Start () {
		weaponName = "smg";
		bullet = Resources.Load("Prefab/Bullet");
	}
		

	public override void fire(Vector3 position, Vector3 velocity, Vector3 target, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack){
		base.fire (position, velocity, target, bulletDelay/4, bulletSpeed, bulletDamage/2.5f, bulletSize/1.5f,bulletKnockBack/3);
	}
}
