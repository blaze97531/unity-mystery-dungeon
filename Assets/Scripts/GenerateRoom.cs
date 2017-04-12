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

//		generateRandomRoom (transform.localPosition);
		Floor floor = new Floor(this, 10, 10);
		floor.generateGameObjects ();
	}
		
	private enum CellEdge {
		EMPTY, WALL, DOOR
	}

	private static CellEdge randomCellEdge (float prob_empty, float prob_wall) {
		float random = Random.value;

		if (random <= prob_empty)
			return CellEdge.EMPTY;
		else if (random <= prob_empty + prob_wall)
			return CellEdge.WALL;
		else
			return CellEdge.DOOR;
	}

	private class Cell {
		public CellEdge pos_x_edge;
		public CellEdge pos_z_edge;
	}

	private class Floor {
		private GenerateRoom enclosingInstance;
		private Cell[,] cells;
		private int n_x, n_z;
		private int[] delta_xs;
		private int[] delta_zs;
		private int[] cumulative_delta_xs; 
		private int[] cumulative_delta_zs;

		public Floor (GenerateRoom enclosingInstance, int n_x, int n_z) {
			this.enclosingInstance = enclosingInstance;
			this.n_x = n_x;
			this.n_z = n_z;
			// Precondition n_x and n_z are both even
			if (n_x % 2 != 0 || n_z % 2 != 0) {
				throw new UnityException ();
			}

			cells = new Cell[n_x,n_z];
			for (int i = 0; i < n_x; i++) {
				for (int j = 0; j < n_z; j++) {
					cells[i, j] = new Cell();
				}
			}


			// Step one: generate a grid with varying delta-x / delta-y
			delta_xs = new int[n_x];
			delta_zs = new int[n_z];
			cumulative_delta_xs = new int[n_x + 1];
			cumulative_delta_zs = new int[n_z + 1];

			cumulative_delta_xs[0] = 0;
			cumulative_delta_zs[0] = 0;

			for (int i = 0; i < n_x; i++) {
				delta_xs[i] = Random.Range(10, 20);
				cumulative_delta_xs[i+1] = delta_xs[i] + cumulative_delta_xs[i];
			}
			for (int i = 0; i < n_z; i++) {
				delta_zs[i] = Random.Range(10, 20);
				cumulative_delta_zs[i+1] = delta_zs[i] + cumulative_delta_zs[i];
			}


			const float EMPTY_CHANCE = 0.3125f; // 5 in 16
			const float WALL_CHANCE = 1.0f - EMPTY_CHANCE;
			// Now generate 2 x 2 segments of the grid.
			for (int i = 0; i < n_x; i += 2) {
				for (int j = 0; j < n_z; j += 2) {
					cells[i, j].pos_x_edge = randomCellEdge(EMPTY_CHANCE, WALL_CHANCE);
					cells[i, j].pos_z_edge = randomCellEdge(EMPTY_CHANCE, WALL_CHANCE);
					cells[i+1, j].pos_z_edge = randomCellEdge(EMPTY_CHANCE, WALL_CHANCE);
					cells[i, j+1].pos_x_edge = randomCellEdge(EMPTY_CHANCE, WALL_CHANCE);

					cells[i+1, j].pos_x_edge = CellEdge.WALL;
					cells[i, j+1].pos_z_edge = CellEdge.WALL;
					cells[i+1, j+1].pos_x_edge = CellEdge.WALL;
					cells[i+1, j+1].pos_z_edge = CellEdge.WALL;
				}
			}

			// Next, need to use Union find to add doors until the entire floor is traversable.
		}

		public void generateGameObjects () {
			int total_x = cumulative_delta_xs [n_x];
			int total_z = cumulative_delta_zs [n_z];

			Vector3 floorCenter = new Vector3 (total_x / 2.0f, 0.0f, total_z / 2.0f);
			GameObject ground = Instantiate<GameObject> (enclosingInstance.groundPrefab, floorCenter, Quaternion.identity, enclosingInstance.transform);
			ground.transform.localScale = new Vector3 (total_x, enclosingInstance.groundPrefab.transform.localScale.y, total_z);

			for (int i = 0; i < n_x; i++) {
				for (int j = 0; j < n_z; j++) {

					if (cells [i, j].pos_x_edge == CellEdge.WALL) {
						Vector3 position = new Vector3 (cumulative_delta_xs [i] + delta_xs [i], enclosingInstance.wallPrefab.transform.position.y, cumulative_delta_zs [j] + delta_zs [j] / 2.0f);
						GameObject wall = Instantiate<GameObject> (enclosingInstance.wallPrefab, position, Quaternion.identity, enclosingInstance.transform);
						wall.transform.localScale = new Vector3 (enclosingInstance.wallPrefab.transform.localScale.x, enclosingInstance.wallPrefab.transform.localScale.y, delta_zs [j]);
					} 

					if (cells [i, j].pos_z_edge == CellEdge.WALL) {
						Vector3 position = new Vector3 (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, enclosingInstance.wallPrefab.transform.position.y, cumulative_delta_zs [j] + delta_zs [j]);
						GameObject wall = Instantiate<GameObject> (enclosingInstance.wallPrefab, position, Quaternion.identity, enclosingInstance.transform);
						wall.transform.localScale = new Vector3 (delta_xs[i], enclosingInstance.wallPrefab.transform.localScale.y, enclosingInstance.wallPrefab.transform.localScale.z);
					} 

				}
			}
		}
	}

	void generateRandomRoom (Vector3 roomCenter) {
		int x_size = Random.Range (5, 10);
		int z_size = Random.Range (5, 10);

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
