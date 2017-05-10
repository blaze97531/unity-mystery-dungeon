using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {
	public Vector3 playerLocation;
	bool map;
	void Start () {
		map = false;
	}
	// Update is called once per frame
	void Update () {
		if (!map) {
			playerLocation = GameObject.Find ("Player").GetComponent<Transform> ().position;
			transform.position = new Vector3 (playerLocation.x, playerLocation.y + 10, playerLocation.z - 5);
			if (Input.GetKeyDown (KeyCode.M)) {
				map = true;
				transform.position = new Vector3 (playerLocation.x, playerLocation.y + 100, playerLocation.z - 5);
				Time.timeScale = 0;
			}
		} else if (Input.GetKeyDown (KeyCode.M)) {
			map = false;
			Time.timeScale = 1;

		}
			

			
	}
}
