using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class machinegun : Weapon {

	void Start () {
		bullet = Resources.Load("Prefab/Bullet");
	}


	public override void fire(Vector3 position, Vector3 velocity, Vector3 target, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack){
		base.fire (position, velocity, target, bulletDelay/3, bulletSpeed, bulletDamage/1.5f, bulletSize*1.25f, bulletKnockBack/1.5f);
	} 
}
