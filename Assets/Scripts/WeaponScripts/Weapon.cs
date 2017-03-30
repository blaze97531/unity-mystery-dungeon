using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
	public string weaponName;
	public Object bullet;

	void Start () {
		weaponName = "pistol";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public virtual float getBulletDelay(float bulletDelay){
		return bulletDelay;
	}

	public virtual void fire(float bulletSpeed, float bulletDamage, float bulletSize, Vector3 position){ //possibly takes weapon as an argument, or make unique "fire" methods for each type of weapon.
		Vector3 direction = getDirection();
		GameObject bul = (GameObject)Instantiate (bullet, position + direction, Quaternion.identity); //transform.position + direction so the bullet starts in front of the player
		bul.transform.localScale = bul.transform.localScale * bulletSize; //scale the size of the bullet.  The bullet's collider should also scale with this
		bul.GetComponent<BulletScript> ().setDamage (bulletDamage);
		bul.GetComponent<BulletScript> ().setSpeed (bulletSpeed);
		Rigidbody rb = bul.GetComponent<Rigidbody> ();
		rb.AddForce (direction * bulletSpeed);
	}
	public virtual Vector3 getDirection(){
		Vector3 direction = Vector3.zero;
		if (Input.GetKey (KeyCode.UpArrow)) {
			if (Input.GetKey (KeyCode.A) && !Input.GetKey (KeyCode.D)) { //If you're holding both, the bullet should go straight
				direction = new Vector3(-.3f,0,1); //I'm experiments with either (-.3f,0,1) or (-.3f,0,0.7f).  (-.3f,0,1) seems more natural, because the gun should always fires forwards with a velocity of 1
			} else if (Input.GetKey (KeyCode.D) && !Input.GetKey (KeyCode.A)) {
				direction = new Vector3(.3f,0,1); //0.3f feels right, this could be changed though.  We could also scale it with movement speed.
			} else {
				direction = Vector3.forward;
			}
		}
		else if (Input.GetKey (KeyCode.DownArrow)) {
			if (Input.GetKey (KeyCode.A) && !Input.GetKey (KeyCode.D)) { 
				direction = new Vector3(-.3f,0,-1);
			} else if (Input.GetKey (KeyCode.D) && !Input.GetKey (KeyCode.A)) {
				direction = new Vector3(.3f,0,-1);
			} else {
				direction = Vector3.back;
			}
		}
		else if (Input.GetKey (KeyCode.LeftArrow)) {
			if (Input.GetKey (KeyCode.W) && !Input.GetKey (KeyCode.S)) { 
				direction = new Vector3(-1,0,.3f);
			} else if (Input.GetKey (KeyCode.S) && !Input.GetKey (KeyCode.W)) {
				direction = new Vector3(-1,0,-.3f);
			} else {
				direction = Vector3.left;
			}
		}
		else if (Input.GetKey (KeyCode.RightArrow)) {
			if (Input.GetKey (KeyCode.W) && !Input.GetKey (KeyCode.S)) { 
				direction = new Vector3(1,0,.3f);
			} else if (Input.GetKey (KeyCode.S) && !Input.GetKey (KeyCode.W)) {
				direction = new Vector3(1,0,-.3f);
			} else {
				direction = Vector3.right;
			}
		}
		return direction;
	}
}
