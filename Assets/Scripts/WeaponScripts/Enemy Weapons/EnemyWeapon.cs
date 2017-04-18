using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {
	public string weaponName;
	public Object bullet;

	void Start () {
		weaponName = "ForwardsShot";
		bullet = Resources.Load("Prefab/EnemyBullet");
	}

	public virtual float getBulletDelay(float bulletDelay){
		return bulletDelay;
	}

	public virtual void fire(float bulletSpeed, float bulletDamage, float bulletSize, Vector3 position, Vector3 direction){ 
		GameObject bul = (GameObject)Instantiate (bullet, position, Quaternion.identity); 
		bul.transform.localScale = bul.transform.localScale * bulletSize;
		bul.GetComponent<EnemyBulletScript> ().setDamage (bulletDamage);
		Rigidbody rb = bul.GetComponent<Rigidbody> ();
		rb.AddForce (direction * bulletSpeed);
	}
}
