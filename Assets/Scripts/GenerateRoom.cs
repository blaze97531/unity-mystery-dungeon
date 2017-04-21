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

		Floor floor = new Floor(this, 10, 10);
		floor.generateGameObjects ();
	}
		
	private enum CellEdge {
		EMPTY, WALL, DOOR
	}

	private enum Direction {
		X, Z
	}

	private static CellEdge RandomCellEdge (float prob_empty, float prob_wall) {
		float random = Random.value;

		if (random <= prob_empty)
			return CellEdge.EMPTY;
		else if (random <= prob_empty + prob_wall)
			return CellEdge.WALL;
		else
			return CellEdge.DOOR;
	}

	private static Direction RandomDirection () {
		int random = Random.Range (0, 2);
		if (random == 0)
			return Direction.X;
		else
			return Direction.Z;
	}

	private class Cell {
		public CellEdge pos_x_edge;
		public CellEdge pos_z_edge;
	}

	private class Floor {
		private GenerateRoom enclosingInstance;
		private Cell[,] cells;
		private CellEdge[] negative_x_border;
		private CellEdge[] negative_z_border;
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

			negative_x_border = new CellEdge[n_z];
			for (int i = 0; i < n_z; i++) {
				negative_x_border[i] = CellEdge.WALL;
			}

			negative_z_border = new CellEdge[n_x];
			for (int i = 0; i < n_x; i++) {
				negative_z_border[i] = CellEdge.WALL;
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
					cells[i, j].pos_x_edge = RandomCellEdge(EMPTY_CHANCE, WALL_CHANCE);
					cells[i, j].pos_z_edge = RandomCellEdge(EMPTY_CHANCE, WALL_CHANCE);
					cells[i+1, j].pos_z_edge = RandomCellEdge(EMPTY_CHANCE, WALL_CHANCE);
					cells[i, j+1].pos_x_edge = RandomCellEdge(EMPTY_CHANCE, WALL_CHANCE);

					cells[i+1, j].pos_x_edge = CellEdge.WALL;
					cells[i, j+1].pos_z_edge = CellEdge.WALL;
					cells[i+1, j+1].pos_x_edge = CellEdge.WALL;
					cells[i+1, j+1].pos_z_edge = CellEdge.WALL;
				}
			}

			// Use union find to determine the individual rooms
			UnionFind<Cell> unionFind = new UnionFind<Cell>();
			foreach (Cell c in cells) {
				unionFind.MakeSingleton(c);
			}

			for (int i = 0; i < n_x; i++) {
				for (int j = 0; j < n_z; j++) {
					if (i + 1 < n_x && cells[i,j].pos_x_edge == CellEdge.EMPTY) {
						unionFind.Union(cells[i,j], cells[i+1, j]);
					}
					if (j + 1 < n_z && cells[i,j].pos_z_edge == CellEdge.EMPTY) {
						unionFind.Union(cells[i,j], cells[i,j+1]);
					}
				}
			}
			// Each set within the unionFind structure now represents a room.
			// TODO: Now, need to get the rooms, and do something with that information.


			// Join the rooms by adding doors. Enough doors have been added once the entire structure is a single set.
			while (unionFind.NumberOfSets() > 1) {
				// Attempt to randomly replace a wall with a door.
				int x = Random.Range(0, n_x);
				int z = Random.Range(0, n_z);
				Direction dir = RandomDirection();

				if (dir == Direction.X && x + 1 < n_x && cells[x,z].pos_x_edge == CellEdge.WALL) {
					cells[x,z].pos_x_edge = CellEdge.DOOR;
					unionFind.Union(cells[x,z], cells[x+1,z]);
				} else if (dir == Direction.Z && z + 1 < n_z && cells[x,z].pos_z_edge == CellEdge.WALL) {
					cells[x,z].pos_z_edge = CellEdge.DOOR;
					unionFind.Union(cells[x,z], cells[x,z+1]);
				}
			}
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
						enclosingInstance.CreateWall (cumulative_delta_xs [i] + delta_xs [i], cumulative_delta_zs [j] + delta_zs [j] / 2.0f, Direction.Z, delta_zs [j]);
					} else if (cells [i, j].pos_x_edge == CellEdge.DOOR) {
						enclosingInstance.CreateDoor (cumulative_delta_xs [i] + delta_xs [i], cumulative_delta_zs [j] + delta_zs [j] / 2.0f, Direction.Z, delta_zs [j]);
					}

					if (cells [i, j].pos_z_edge == CellEdge.WALL) {
						enclosingInstance.CreateWall (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, cumulative_delta_zs [j] + delta_zs [j], Direction.X, delta_xs [i]);
					} else if (cells [i, j].pos_z_edge == CellEdge.DOOR) {
						enclosingInstance.CreateDoor (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, cumulative_delta_zs [j] + delta_zs [j], Direction.X, delta_xs [i]);
					}
				}
			}

			for (int i = 0; i < n_z; i++) {
				if (negative_x_border [i] == CellEdge.WALL) {
					enclosingInstance.CreateWall (0.0f, cumulative_delta_zs [i] + delta_zs [i] / 2.0f, Direction.Z, delta_zs [i]);
				} else if (negative_x_border [i] == CellEdge.DOOR) {
					enclosingInstance.CreateDoor (0.0f, cumulative_delta_zs [i] + delta_zs [i] / 2.0f, Direction.Z, delta_zs [i]);
				}
			}

			for (int i = 0; i < n_x; i++) {
				if (negative_z_border [i] == CellEdge.WALL) {
					enclosingInstance.CreateWall (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, 0.0f, Direction.X, delta_xs [i]);
				} else if (negative_z_border [i] == CellEdge.DOOR) {
					enclosingInstance.CreateWall (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, 0.0f, Direction.X, delta_xs [i]);
				}
			}
		}
	}

	private void CreateWall (float x_position, float z_position, Direction longEdge, float longEdgeLength) {
		Vector3 position = new Vector3 (x_position, wallPrefab.transform.position.y, z_position);
		GameObject wall = Instantiate<GameObject> (wallPrefab, position, Quaternion.identity, transform);
		if (longEdge == Direction.X) {
			wall.transform.localScale = new Vector3 (longEdgeLength, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
		} else {
			wall.transform.localScale = new Vector3 (wallPrefab.transform.localScale.x, wallPrefab.transform.localScale.y, longEdgeLength);
		}
	}

	private void CreateDoor (float x_position, float z_position, Direction longEdge, float longEdgeLength) {
		// TODO
	}

	// Very poorly optimized implementation of the UnionFind data structure
	private class UnionFind<T> {
		private List<List<T>> sets;

		public UnionFind () {
			sets = new List<List<T>>();
		}

		/* Makes a new set containing a single element. */
		public void MakeSingleton (T element) {
			List<T> newSet = new List<T> ();
			newSet.Add (element);
			sets.Add (newSet);
		}

		/* Performs the union of the sets containing x and y, if x and y are not already in the same set. */
		public void Union (T x, T y) {
			List<T> xList = Find (x);
			List<T> yList = Find (y);

			if (xList != yList) {
				foreach (T element in yList)
					xList.Add (element);
				sets.Remove (yList);
			}
		}

		/* Returns the set containing the element, or null if the element is not in a set. */
		public List<T> Find (T element) {
			foreach (List<T> s in sets) {
				if (s.Contains(element))
					return s;
			}
			return null;
		}

		/* Returns the sets. */
		public List<List<T>> GetSets () {
			return sets;
		}

		/* Returns the number of sets. */ 
		public int NumberOfSets () {
			return sets.Count;
		}
	}
}
