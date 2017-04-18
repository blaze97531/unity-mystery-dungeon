using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelTurningScript : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			transform.eulerAngles = new Vector3 (270, 180, 0);
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			transform.eulerAngles = new Vector3 (270, 0, 0);
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.eulerAngles = new Vector3 (270, 90, 0);
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			transform.eulerAngles = new Vector3 (270, 270, 0);
		} else {
			if (Input.GetKey (KeyCode.W)) {
				transform.eulerAngles = new Vector3 (270, 180, 0);
			} else if (Input.GetKey (KeyCode.S)) {
				transform.eulerAngles = new Vector3 (270, 0, 0);
			} else if (Input.GetKey (KeyCode.A)) {
				transform.eulerAngles = new Vector3 (270, 90, 0);
			} else if (Input.GetKey (KeyCode.D)) {
				transform.eulerAngles = new Vector3 (270, 270, 0);
			}
		}
	}
}
