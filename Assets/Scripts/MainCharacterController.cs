using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour {
	public float movementSpeed = 7.5f;
	public float bulletSpeed = 500.0f;
	public float bulletKnockBack= 1;
	public float bulletSize = 1f; //possibly scale this with damage
	public float bulletDelay = 0.5f; //at 0, the gun fires every frame
	public float bulletDamage = 5f;
	public Weapon weapon;
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
			if (currTime >= (weapon.getBulletDelay(bulletDelay))) { //my current formula for bullet delay
				weapon.fire (bulletSpeed, bulletDamage, bulletSize, bulletKnockBack, transform.position);
				currTime = 0;
			}
		}
	}
}