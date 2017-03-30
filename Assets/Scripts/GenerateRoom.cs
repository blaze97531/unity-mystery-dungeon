using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRoom : MonoBehaviour {

	private GameObject groundPrefab;
	private GameObject wallPrefab;
	private GameObject emptyPrefab;

	// Use this for initialization
	void Start () {
		groundPrefab = Resources.Load<GameObject> ("Prefab/Ground");
		wallPrefab = Resources.Load<GameObject> ("Prefab/Wall");
		emptyPrefab = Resources.Load<GameObject> ("Prefab/EmptyGameObject");

		generateRandomRoom (transform.localPosition);
	}

	void generateRandomRoom (Vector3 roomCenter) {
		int x_size = Random.Range (10, 30);
		int z_size = Random.Range (10, 30);

		GameObject ground = Instantiate<GameObject> (groundPrefab, roomCenter, Quaternion.identity, transform);
		ground.transform.localScale = new Vector3 (x_size, groundPrefab.transform.localScale.y, z_size);

		GameObject positiveXWall = Instantiate<GameObject> (wallPrefab, roomCenter, Quaternion.identity, transform);
		positiveXWall.transform.Translate (x_size / 2 * Vector3.right);
		positiveXWall.transform.localScale = new Vector3 (wallPrefab.transform.localScale.x, wallPrefab.transform.localScale.y, z_size);

		GameObject negativeXWall = Instantiate<GameObject> (wallPrefab, roomCenter, Quaternion.identity, transform);
		negativeXWall.transform.Translate (x_size / 2 * Vector3.left);
		negativeXWall.transform.localScale = new Vector3 (wallPrefab.transform.localScale.x, wallPrefab.transform.localScale.y, z_size);

		GameObject positiveZWall = Instantiate<GameObject> (wallPrefab, roomCenter, Quaternion.identity, transform);
		positiveZWall.transform.Translate (z_size / 2 * Vector3.forward);
		positiveZWall.transform.localScale = new Vector3 (x_size, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);

		GameObject negativeZWall = Instantiate<GameObject> (wallPrefab, roomCenter, Quaternion.identity, transform);
		negativeZWall.transform.Translate (z_size / 2 * Vector3.back);
		negativeZWall.transform.localScale = new Vector3 (x_size, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
