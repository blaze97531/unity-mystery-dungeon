using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public Vector3 playerLocation;
	bool map;

	private const float CAMERA_SPEED_DEAD = 10.0f;
	void Start () {
		map = false;
	}
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.FindWithTag ("Player");
		if (player != null) {
			playerLocation = GameObject.FindWithTag ("Player").GetComponent<Transform> ().position;
			transform.position = new Vector3 (playerLocation.x, transform.position.y, playerLocation.z - 2);

			if (Input.GetKey (KeyCode.Equals) && transform.position.y >= 10.0f) {
				transform.Translate (Vector3.down * CAMERA_SPEED_DEAD * Time.deltaTime, Space.World);
			} 
			if (Input.GetKey (KeyCode.Minus) && transform.position.y <= 20.0f) {
				transform.Translate (Vector3.up * CAMERA_SPEED_DEAD * Time.deltaTime, Space.World);
			}

		} else {
			/* Player is dead, can move around freely. */
			if (Input.GetKey (KeyCode.W)) {
				transform.Translate (Vector3.forward * CAMERA_SPEED_DEAD * Time.deltaTime, Space.World);
			} 
			if (Input.GetKey (KeyCode.A)) {
				transform.Translate (Vector3.left * CAMERA_SPEED_DEAD * Time.deltaTime, Space.World);
			} 
			if (Input.GetKey (KeyCode.S)) {
				transform.Translate (Vector3.back * CAMERA_SPEED_DEAD * Time.deltaTime, Space.World);
			} 
			if (Input.GetKey (KeyCode.D)) {
				transform.Translate (Vector3.right * CAMERA_SPEED_DEAD * Time.deltaTime, Space.World);
			} 
			if (Input.GetKey (KeyCode.Equals)) {
				transform.Translate (Vector3.down * CAMERA_SPEED_DEAD * Time.deltaTime, Space.World);
			} 
			if (Input.GetKey (KeyCode.Minus)) {
				transform.Translate (Vector3.up * CAMERA_SPEED_DEAD * Time.deltaTime, Space.World);
			}
		}
	}
}
