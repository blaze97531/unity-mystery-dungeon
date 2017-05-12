using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
	public string weaponName;
	public Object bullet;

	protected float currTime = 0;

	void Start () {
		weaponName = "pistol";
		bullet = Resources.Load("Prefab/Bullet");
	}

	public virtual float getBulletDelay(float bulletDelay){
		return bulletDelay;
	}

	public virtual void fire(Vector3 velocity, float bulletDelay, float bulletSpeed, float bulletDamage, float bulletSize, float bulletKnockBack, Vector3 position){ 
		currTime = currTime + Time.deltaTime;
		if (currTime >= bulletDelay	&& (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.LeftArrow))) {
			Vector3 direction = velocity + getDirection();
			GameObject bul = (GameObject)Instantiate (bullet, position + getGunLocationOffset(), Quaternion.identity); //transform.position + direction so the bullet starts in front of the player
			bul.transform.localScale = bul.transform.localScale * bulletSize; //scale the size of the bullet.  The bullet's collider should also scale with this
			bul.GetComponent<BulletScript> ().setDamage (bulletDamage);
			bul.GetComponent<BulletScript> ().setKnockBack (bulletKnockBack);
			bul.GetComponent<BulletScript> ().setDirection (direction);
			Rigidbody rb = bul.GetComponent<Rigidbody> ();
			rb.AddForce (direction * bulletSpeed);
			currTime = 0;
		}
	}

	public virtual Vector3 getDirection(){
		Vector3 direction = Vector3.zero;
		if (Input.GetKey (KeyCode.UpArrow)) {
			direction = Vector3.forward;
		}
		else if (Input.GetKey (KeyCode.DownArrow)) {
			direction = Vector3.back;
		}
		else if (Input.GetKey (KeyCode.LeftArrow)) {
			direction = Vector3.left;
		}
		else if (Input.GetKey (KeyCode.RightArrow)) {
			direction = Vector3.right;
		}
		return direction;
	}

	public virtual Vector3 getGunLocationOffset(){
		Vector3 offset = Vector3.zero;
		if (Input.GetKey (KeyCode.UpArrow)) {
			offset = new Vector3 (-0.35f, 0.65f, 1);
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			offset = new Vector3 (0.35f, 0.65f, -1);
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			offset = new Vector3 (-1, 0.65f, -0.35f);
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			offset = new Vector3 (1, 0.65f, 0.35f);
		}
		return offset;
	}
}
