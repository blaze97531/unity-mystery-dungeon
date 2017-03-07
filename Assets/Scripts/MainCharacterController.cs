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
		if (Input.GetKey (KeyCode.UpArrow)) {
			GameObject bul = (GameObject) Instantiate (bullet, transform.position, Quaternion.identity);
			Rigidbody rb = bul.GetComponent<Rigidbody>();
			rb.AddForce(Vector3.forward*bulletspeed);
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			GameObject bul = (GameObject) Instantiate (bullet, transform.position, Quaternion.identity);
			Rigidbody rb = bul.GetComponent<Rigidbody>();
			rb.AddForce(Vector3.back*bulletspeed);
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			GameObject bul = (GameObject) Instantiate (bullet, transform.position, Quaternion.identity);
			Rigidbody rb = bul.GetComponent<Rigidbody>();
			rb.AddForce(Vector3.left*bulletspeed);
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			GameObject bul = (GameObject) Instantiate (bullet, transform.position, Quaternion.identity);
			Rigidbody rb = bul.GetComponent<Rigidbody>();
			rb.AddForce(Vector3.right*bulletspeed);
		}

	}
}
