using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraControlScript : MonoBehaviour {

	private bool mapShown = false;

	void Update () {
		GameObject ground = GameObject.FindWithTag ("Ground");
		Camera camera = GetComponent<Camera> ();
		if (ground != null) {
			float a = 2.0f * Mathf.Atan (camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
			float camera_distance_z = ground.transform.localScale.z / a;
			float camera_distance_x = ground.transform.localScale.x / camera.aspect / a;
			camera.transform.position = new Vector3 (ground.transform.position.x, Mathf.Max (camera_distance_x, camera_distance_z), ground.transform.position.z);
		}

		if (Input.GetKeyDown (KeyCode.M)) {
			mapShown = !mapShown;
			if (mapShown) {
				camera.rect = new Rect (0.1f, 0.1f, 0.8f, 0.8f);
			} else {
				camera.rect = new Rect (0.0f, 0.0f, 0.0f, 0.0f);
			}
		}
		
	}
}
