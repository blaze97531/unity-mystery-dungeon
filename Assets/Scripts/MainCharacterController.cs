using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour {
	public float movementSpeed = 7.5f;
	public float bulletSpeed = 600.0f;
	public float bulletSize = 1.5f; //possibly scale this with damage
	public float bulletDelay = 0.1f; //at 0, the gun fires every frame
	public Object bullet;
	private float currTime = 0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W)) {
			transform.Translate (Time.deltaTime * movementSpeed * Vector3.forward);
		}
		if (Input.GetKey (KeyCode.S)) {
			transform.Translate (Time.deltaTime * movementSpeed * Vector3.back);
		}
		if (Input.GetKey (KeyCode.A)) {
			transform.Translate (Time.deltaTime * movementSpeed * Vector3.left);
		}
		if (Input.GetKey (KeyCode.D)) {
			transform.Translate (Time.deltaTime * movementSpeed * Vector3.right);
		}
		currTime = currTime + Time.deltaTime;
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.RightArrow)||Input.GetKey (KeyCode.LeftArrow)) {
			if (currTime >= (1*bulletDelay)) { //my current formula for bullet delay
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
		GameObject bul = (GameObject) Instantiate (bullet, transform.position, Quaternion.identity); //change position to be in front of the player
		bul.transform.localScale = bul.transform.localScale * bulletSize; //scale the size of the bullet.  The bullet's collider should also scale with this
		Rigidbody rb = bul.GetComponent<Rigidbody>();
		rb.AddForce(direction*bulletSpeed);
	}
}
