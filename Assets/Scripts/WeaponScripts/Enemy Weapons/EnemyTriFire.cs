using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriFire : EnemyWeapon {

	void Start () {
		weaponName = "TriFire";
		bullet = Resources.Load("Prefab/EnemyBullet");
	}

	public override void fire(float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, Vector3 position, Vector3 direction){ 
		for (int i = -1; i <= 1; i++) { //essentially fires 5 bullets in an arch
			GameObject bul = (GameObject)Instantiate (bullet, position, Quaternion.identity);
			bul.transform.localScale = bul.transform.localScale * bulletSize;
			bul.GetComponent<EnemyBulletScript> ().setDamage (bulletDamage);
			Rigidbody rb = bul.GetComponent<Rigidbody> ();
			Vector3 dir = Quaternion.AngleAxis (15*i,Vector3.up) * direction;
			rb.AddForce (dir * bulletSpeed);
		}
	}
}
