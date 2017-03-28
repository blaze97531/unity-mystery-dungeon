using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour {
	public float movementSpeed = 7.5f;
	public float bulletSpeed = 600.0f;
	public float bulletSize = 1.5f; //possibly scale this with damage
	public float bulletDelay = 0.1f; //at 0, the gun fires every frame
	public float bulletDamage = 1f;
	public Object bullet;
	public string weapon = "pistol"; //perhaps turn this into an enum in the future.  Lets keep track of each weapon in the game ideas google doc
	//pistol, smg, sniper, slug, shotgun, dual
	private float currTime = 0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W)) {
			transform.Translate (Time.deltaTime * getMovementSpeed() * Vector3.forward);
		}
		if (Input.GetKey (KeyCode.S)) {
			transform.Translate (Time.deltaTime * getMovementSpeed() * Vector3.back);
		}
		if (Input.GetKey (KeyCode.A)) {
			transform.Translate (Time.deltaTime * getMovementSpeed() * Vector3.left);
		}
		if (Input.GetKey (KeyCode.D)) {
			transform.Translate (Time.deltaTime * getMovementSpeed() * Vector3.right);
		}
		currTime = currTime + Time.deltaTime;
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.RightArrow)||Input.GetKey (KeyCode.LeftArrow)) {
			if (currTime >= (1*getBulletDelay())) { //my current formula for bullet delay
				fire ();
				currTime = 0;
			}
		}
	}

	void fire(){ //possibly takes weapon as an argument, or make unique "fire" methods for each type of weapon.
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
		if (weapon == "shotgun") {
			for (int i = -2; i <= 2; i++) { //essentially fires 5 bullets in an arch
				GameObject bul = (GameObject)Instantiate (bullet, transform.position + direction, Quaternion.identity);
				bul.transform.localScale = bul.transform.localScale * getBulletSize ();
				bul.GetComponent<BulletScript> ().setDamage (getBulletDamage ());
				bul.GetComponent<BulletScript> ().setSpeed (getBulletSpeed ());
				Rigidbody rb = bul.GetComponent<Rigidbody> ();
				if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow)) {	//I cant say this math entirely checks out, but it actually looks really nice in game
					rb.AddForce (new Vector3 (direction.x + Mathf.Sin (i * Mathf.PI / 28), 0, direction.z * Mathf.Cos (i * Mathf.PI / 12)) * getBulletSpeed ());
				} else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.LeftArrow)) {
					rb.AddForce (new Vector3 (direction.x * Mathf.Cos (i * Mathf.PI / 12), 0, direction.z +  Mathf.Sin (i * Mathf.PI / 28)) * getBulletSpeed ());
				}
			}
		} else if (weapon == "dual") {
			for (float i = -0.25f; i <= 0.25f; i = i + 0.5f) { 
				Vector3 position = transform.position;
				if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow)) {
					position.x = position.x + i;
				} else if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.LeftArrow)) {
					position.z = position.z + i;
				}
				GameObject bul = (GameObject)Instantiate (bullet, position + direction, Quaternion.identity); //transform.position + direction so the bullet starts in front of the player
				bul.transform.localScale = bul.transform.localScale * getBulletSize (); //scale the size of the bullet.  The bullet's collider should also scale with this
				bul.GetComponent<BulletScript> ().setDamage (getBulletDamage ());
				bul.GetComponent<BulletScript> ().setSpeed (getBulletSpeed ());
				Rigidbody rb = bul.GetComponent<Rigidbody> ();
				rb.AddForce (direction * getBulletSpeed ());
			}
		} else {
			GameObject bul = (GameObject)Instantiate (bullet, transform.position + direction, Quaternion.identity); //transform.position + direction so the bullet starts in front of the player
			bul.transform.localScale = bul.transform.localScale * getBulletSize (); //scale the size of the bullet.  The bullet's collider should also scale with this
			bul.GetComponent<BulletScript> ().setDamage (getBulletDamage ());
			bul.GetComponent<BulletScript> ().setSpeed (getBulletSpeed ());
			Rigidbody rb = bul.GetComponent<Rigidbody> ();
			rb.AddForce (direction * getBulletSpeed ());
		}
	}

	//Call these methods, and depending on your weapons, we can return modified stats.  This makes it easier to balance weapons
	//Passive items will directly affect stats (damage up would be bulletDamage = bulletDamage + x)
	//Weapons will not directly affect stats
	//When we display the stats in the UI, I am unsure if we should show the results of these methods, or the base stats. probably the latter.
	//Weapon stats are layed out in the game ideas google doc.  Make sure to keep the document up to date
	private float getBulletSpeed(){ 
		if(weapon == "sniper")
			return bulletSpeed * 2;
		if(weapon == "slug")
			return bulletSpeed / 1.25f;
		else
			return bulletSpeed;
	}

	private float getBulletSize(){
		if (weapon == "slug")
			return bulletSize * 3;
		else
			return bulletSize;
	}

	private float getBulletDelay(){
		if(weapon == "sniper")
			return bulletDelay*3;
		else if(weapon == "smg")
			return bulletDelay/3;
		else if(weapon == "dual")
			return bulletDelay*1.5f;
		else if(weapon == "shotgun")
			return bulletDelay*3;
		else if(weapon == "slug")
			return bulletDelay*5;
		else
			return bulletDelay;
	}

	private float getBulletDamage(){
		if(weapon == "sniper")
			return bulletDamage*3;
		else if(weapon == "smg")
			return bulletDamage/3;
		else if(weapon == "dual")
			return bulletDamage/1.5f;
		else if(weapon == "slug")
			return bulletDamage*5;
		else
			return bulletDamage;
	}

	private float getMovementSpeed(){
		return movementSpeed;
	}
}
