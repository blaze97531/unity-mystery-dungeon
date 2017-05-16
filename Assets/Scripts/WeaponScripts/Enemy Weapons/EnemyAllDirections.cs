using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAllDirections : EnemyWeapon {

	void Start () {
		weaponName = "AllDirections";
		bullet = Resources.Load("Prefab/Weapons/EnemyBullet");
	}

	public override void fire(float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, Vector3 position, Vector3 direction){ 
		float rand = Random.Range(0,18);
		for (int i = 0; i < 20; i++) { //essentially fires 5 bullets in an arch
			GameObject bul = (GameObject)Instantiate (bullet, position, Quaternion.identity);
			bul.transform.localScale = bul.transform.localScale * bulletSize;
			bul.GetComponent<EnemyBulletScript> ().setDamage (bulletDamage);
			Rigidbody rb = bul.GetComponent<Rigidbody> ();
			Vector3 dir = Quaternion.AngleAxis ((18*i) + rand,Vector3.up) * direction;
			rb.AddForce (dir * bulletSpeed);
		}
	}
}
