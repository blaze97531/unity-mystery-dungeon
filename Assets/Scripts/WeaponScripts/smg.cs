using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smg : Weapon {

	void Start () {
		weaponName = "smg";
		bullet = Resources.Load("Prefab/Bullet");
	}
		
	public override void fire(float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack, Vector3 position){
		base.fire (bulletDelay/4, bulletSpeed, bulletDamage/2.5f, bulletSize/1.5f,bulletKnockBack/3, position);
	}
}
