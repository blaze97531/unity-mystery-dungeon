using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will carry the bullet's properties, such as damage, speed (for calculating knockback), and other affects (slow, poison, peircing, ect.)
public class BulletScript : MonoBehaviour {
	private float damage;
	private float knockBack; //this is for determining knockback, for whenever we decide to implement that
	private Vector3 direction;

	private AudioSource gunShot;

	private void Start () {
		gunShot = GetComponent<AudioSource> ();
		gunShot.Play ();
	}

	public void setDirection(Vector3 dir){
		direction = dir;
	}
	public Vector3 getDirection(){
		return direction;
	}
	//would you rather use getters and setters? or just have public variables?
	public void setDamage(float bulletDamage){
		damage = bulletDamage;
	}

	public float getDamage(){
		return damage;
	}

	public void setKnockBack(float bulletKnockBack) {
		knockBack = bulletKnockBack;
	}

	public float getKnockBack() {
		return knockBack;
	}

}
