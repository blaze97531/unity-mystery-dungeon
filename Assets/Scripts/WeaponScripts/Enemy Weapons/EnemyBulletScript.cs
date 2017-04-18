using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will carry the bullet's properties, such as damage, speed (for calculating knockback), and other affects (slow, poison, peircing, ect.)
public class EnemyBulletScript : MonoBehaviour {
	public float damage;

	public void setDamage(float bulletDamage){
		damage = bulletDamage;
	}

	public float getDamage(){
		return damage;
	}

}
