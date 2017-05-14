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
		playerLocation = GameObject.FindWithTag ("Player").GetComponent<Transform> ().position;
		transform.position = new Vector3 (playerLocation.x, playerLocation.y + 10, playerLocation.z - 5);
	}
}
