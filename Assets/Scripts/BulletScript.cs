using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will carry the bullet's properties, such as damage, speed (for calculating knockback), and other affects (slow, poison, peircing, ect.)
public class BulletScript : MonoBehaviour {
	private float damage;
	private float speed; //this is for determining knockback, for whenever we decide to implement that

	//would you rather use getters and setters? or just have public variables?
	public void setDamage(float bulletDamage){
		damage = bulletDamage;
	}

	public float getDamage(){
		return damage;
	}

	public void setSpeed(float bulletSpeed){
		speed = bulletSpeed;
	}

	public float getSpeed(){
		return speed;
	}

}
