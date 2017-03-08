using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour {
	public float speed = 10.0f;
	public float bulletspeed = 500.0f;
	public Object bullet;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W)) {
			transform.Translate (Time.deltaTime * speed * Vector3.forward);
		}
		if (Input.GetKey (KeyCode.S)) {
			transform.Translate (Time.deltaTime * speed * Vector3.back);
		}
		if (Input.GetKey (KeyCode.A)) {
			transform.Translate (Time.deltaTime * speed * Vector3.left);
		}
		if (Input.GetKey (KeyCode.D)) {
			transform.Translate (Time.deltaTime * speed * Vector3.right);
		}
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.RightArrow)||Input.GetKey (KeyCode.LeftArrow)) {
			fire ();
		}
}

	void fire(){
		Vector3 direction = Vector3.zero;
		if (Input.GetKey (KeyCode.UpArrow)) {
			if (Input.GetKey (KeyCode.A)) {
				direction = new Vector3(-.25f,0,.75f);
			} else if (Input.GetKey (KeyCode.D)) {
				direction = new Vector3(.25f,0,.75f);
			} else {
				direction = Vector3.forward;
			}
		}
		else if (Input.GetKey (KeyCode.DownArrow)) {
			direction =  Vector3.back;
		}
		else if (Input.GetKey (KeyCode.LeftArrow)) {
			direction = Vector3.left;
		}
		else if (Input.GetKey (KeyCode.RightArrow)) {
			direction = Vector3.right;
		}
		GameObject bul = (GameObject) Instantiate (bullet, transform.position, Quaternion.identity);
		Rigidbody rb = bul.GetComponent<Rigidbody>();
		rb.AddForce(direction*bulletspeed);
	}
}
