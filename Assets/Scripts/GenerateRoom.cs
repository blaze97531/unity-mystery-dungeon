using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRoom : MonoBehaviour {

	private GameObject groundPrefab;
	private GameObject wallPrefab;
	private GameObject emptyPrefab;
	private GameObject singleDoorPrefab;

	private GameObject[] enemyPrefabs;

	private GameObject player;

	// Use this for initialization
	void Start () {
		groundPrefab = Resources.Load<GameObject> ("Prefab/Ground");
		wallPrefab = Resources.Load<GameObject> ("Prefab/Wall");
		emptyPrefab = Resources.Load<GameObject> ("Prefab/EmptyGameObject");
		singleDoorPrefab = Resources.Load<GameObject> ("Prefab/SingleDoor");

		enemyPrefabs = Resources.LoadAll<GameObject> ("Prefab/Enemies");

		Floor floor = new Floor(this, 10, 10);
		floor.generateGameObjects ();

		player = GameObject.Find ("Player");
	}

	void Update () {
		
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

	private class Room {
		// The grid cells of the floor that comprise this room
		public List<Cell> cells; 

		// All the doors leading in and out of this room. Note: 
		// individual doors will appear in TWO different rooms
		public List<GameObject> doors;

		public List<GameObject> enemiesToSpawn;
		public List<Vector3> spawnPositions;

		public List<GameObject> activeEnemies;
		public bool cleared;

		public Room (List<Cell> cells) {
			this.cells = cells;
			this.doors = new List<GameObject>();
			enemiesToSpawn = new List<GameObject>();
			spawnPositions = new List<Vector3>();
			cleared = false;
		}

		public void AddDoor (GameObject door) {
			this.doors.Add (door);
		}

		public void AddEnemyToSpawn (GameObject enemy, Vector3 spawnPosition) {
			enemiesToSpawn.Add (enemy);
			spawnPositions.Add (spawnPosition);
		}

		public void OpenAllDoors () {
			foreach (GameObject door in doors) {
				door.GetComponent<DoorControlScript> ().OpenDoor ();
			}
			SpawnEnemies ();
		}

		public void CloseAllDoors () {
			foreach (GameObject door in doors) {
				door.GetComponent<DoorControlScript> ().CloseDoor ();
			}
		}

		private void SpawnEnemies () {
			for (int i = 0; i < enemiesToSpawn.Count; i++) {
				GameObject newEnemy = Instantiate<GameObject> (enemiesToSpawn [i], spawnPositions [i], Quaternion.identity);
			}
			enemiesToSpawn.Clear ();
			spawnPositions.Clear ();
		}
			
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

		// UnionFind structure where each set is all cells that make up one room.
		private UnionFind<Cell> rooms;
		// List<Cells> in a room --> Room object for that room
		private Dictionary<List<Cell>, Room> cellsToRooms;

		public Floor (GenerateRoom enclosingInstance, int n_x, int n_z) {
			this.cellsToRooms = new Dictionary<List<Cell>, Room>();
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
				delta_xs[i] = Random.Range(10, 15);
				cumulative_delta_xs[i+1] = delta_xs[i] + cumulative_delta_xs[i];
			}
			for (int i = 0; i < n_z; i++) {
				delta_zs[i] = Random.Range(10, 15);
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
			// Copy the current state of the unionFind structure; this copy should not be modified further.
			rooms = unionFind.Copy();
			foreach (List<Cell> cellList in rooms.GetSets()) {
				cellsToRooms.Add(cellList, new Room(cellList));
			}

			// Probability of adding an unncessary door (a door that connects two sections that are already connected).
			float probUnnecessaryDoor = 0.25f; 
			// Join the rooms by adding doors. Enough doors have been added once the entire structure is a single set.
			while (unionFind.NumberOfSets() > 1) {
				// Attempt to randomly replace a wall with a door.
				int x = Random.Range(0, n_x);
				int z = Random.Range(0, n_z);
				Direction dir = RandomDirection();

				// However, don't replace a wall with a door if the two cells are already in the same room.
				if (dir == Direction.X && x + 1 < n_x && !rooms.InSameSet(cells[x,z], cells[x+1,z]) && cells[x,z].pos_x_edge == CellEdge.WALL && (!unionFind.InSameSet(cells[x,z], cells[x+1,z]) || Random.value <= probUnnecessaryDoor)) {
					cells[x,z].pos_x_edge = CellEdge.DOOR;
					unionFind.Union(cells[x,z], cells[x+1,z]);
				} else if (dir == Direction.Z && z + 1 < n_z && !rooms.InSameSet(cells[x,z], cells[x,z+1]) && cells[x,z].pos_z_edge == CellEdge.WALL && (!unionFind.InSameSet(cells[x,z], cells[x,z+1]) || Random.value <= probUnnecessaryDoor)) {
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
						GameObject[] doors = enclosingInstance.CreateDoor (cumulative_delta_xs [i] + delta_xs [i], cumulative_delta_zs [j] + delta_zs [j] / 2.0f, Direction.Z, delta_zs [j]);
						AddDoorsToRooms (doors, cells [i, j], cells [i + 1, j]);
					}

					if (cells [i, j].pos_z_edge == CellEdge.WALL) {
						enclosingInstance.CreateWall (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, cumulative_delta_zs [j] + delta_zs [j], Direction.X, delta_xs [i]);
					} else if (cells [i, j].pos_z_edge == CellEdge.DOOR) {
						GameObject[] doors = enclosingInstance.CreateDoor (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, cumulative_delta_zs [j] + delta_zs [j], Direction.X, delta_xs [i]);
						AddDoorsToRooms (doors, cells [i, j], cells [i, j+1]);
					}
				}
			}

			for (int i = 0; i < n_z; i++) {
				if (negative_x_border [i] == CellEdge.WALL) {
					enclosingInstance.CreateWall (0.0f, cumulative_delta_zs [i] + delta_zs [i] / 2.0f, Direction.Z, delta_zs [i]);
				} else if (negative_x_border [i] == CellEdge.DOOR) {
					GameObject[] doors = enclosingInstance.CreateDoor (0.0f, cumulative_delta_zs [i] + delta_zs [i] / 2.0f, Direction.Z, delta_zs [i]);
					// TODO: will need to add the doors to the Room objects, although this code cannot currently be executed.
				}
			}

			for (int i = 0; i < n_x; i++) {
				if (negative_z_border [i] == CellEdge.WALL) {
					enclosingInstance.CreateWall (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, 0.0f, Direction.X, delta_xs [i]);
				} else if (negative_z_border [i] == CellEdge.DOOR) {
					GameObject[] doors = enclosingInstance.CreateDoor (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, 0.0f, Direction.X, delta_xs [i]);
					// TODO: will need to add the doors to the Room objects, although this code cannot currently be executed.
				}
			}

			for (int i = 0; i < n_x; i++) {
				for (int j = 0; j < n_z; j++) {
					Room r;
					cellsToRooms.TryGetValue (rooms.Find(cells[i,j]), out r);

					int enemies_to_spawn = Random.Range(0, 3);
					for (int k = enemies_to_spawn; k > 0; k--) {
						GameObject enemyToSpawn = enclosingInstance.enemyPrefabs [Random.Range (0, enclosingInstance.enemyPrefabs.Length)];

						Vector3 spawnPosition = new Vector3 (cumulative_delta_xs [i] + Random.Range(0.0f, delta_xs[i]), enemyToSpawn.transform.position.y, cumulative_delta_zs [j] + Random.Range(0.0f, delta_zs[j]));

						// TODO: Major problem ensure this doesn't intersect with anything else in the room.
						r.AddEnemyToSpawn (enemyToSpawn, spawnPosition);
					}
				}
			}

			/* DEBUGGING Purposes: Open all doors on the floor */
			foreach (Room r in cellsToRooms.Values) {
				r.OpenAllDoors ();
			}
		}

		private void AddDoorsToRooms(GameObject[] doorsToAdd, Cell cellInRoom1, Cell cellInRoom2) {
			Room room1, room2;
			cellsToRooms.TryGetValue (rooms.Find (cellInRoom1), out room1);
			cellsToRooms.TryGetValue (rooms.Find (cellInRoom2), out room2);

			foreach (GameObject door in doorsToAdd) {
				room1.AddDoor (door);
				room2.AddDoor (door);
			}
		}


		/* Returns the cell that contains the given (x,z) position. Returns null if the position is out of bounds of the floor. */
		private Cell GetCellContaining(float x, float z) {
			if (x < 0 || z < 0 || x >= cumulative_delta_xs [n_x] || z >= cumulative_delta_zs [n_z]) {
				return null;
			} else {
				int x_index = 0; 
				int z_index = 0;
				while (cumulative_delta_xs [x_index] + delta_xs[x_index] < x) {
					x_index++;
				}
				while (cumulative_delta_zs [z_index] + delta_zs[z_index] < z) {
					z_index++;
				}
				return cells [x_index, z_index];
			}
		}
	} // End Floor class

	private void CreateWall (float x_position, float z_position, Direction longEdge, float longEdgeLength) {
		Vector3 position = new Vector3 (x_position, wallPrefab.transform.position.y, z_position);
		GameObject wall = Instantiate<GameObject> (wallPrefab, position, Quaternion.identity, transform);
		if (longEdge == Direction.X) {
			wall.transform.localScale = new Vector3 (longEdgeLength, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
		} else {
			wall.transform.localScale = new Vector3 (wallPrefab.transform.localScale.x, wallPrefab.transform.localScale.y, longEdgeLength);
		}
	}

	/* This function actually spawns a door prefab. It returns the door it spawns. */
	private GameObject SpawnDoorPrefab (float x_position, float z_position, Direction longEdge, float longEdgeLength) {
		Vector3 position = new Vector3 (x_position, singleDoorPrefab.transform.position.y, z_position);
		GameObject door = Instantiate<GameObject> (singleDoorPrefab, position, Quaternion.identity, transform);
		if (longEdge == Direction.X) {
			door.transform.localScale = new Vector3 (longEdgeLength, singleDoorPrefab.transform.localScale.y, singleDoorPrefab.transform.localScale.z);
		} else {
			door.transform.localScale = new Vector3 (singleDoorPrefab.transform.localScale.x, singleDoorPrefab.transform.localScale.y, longEdgeLength);
		}
		return door;
	}

	/* This function creates the two walls and two single doors that make up the entire section of the wall where there is a door. 
	 * It returns an array of the two single door objects that where created. */
	private GameObject[] CreateDoor (float x_position, float z_position, Direction longEdge, float longEdgeLength) {
		
		float DOOR_WIDTH = 2.5f; // The width of the entire door opening.
		if (longEdge == Direction.X) {
			CreateWall(x_position - (longEdgeLength + DOOR_WIDTH) / 4.0f, z_position, Direction.X, (longEdgeLength - DOOR_WIDTH) / 2.0f);
			CreateWall(x_position + (longEdgeLength + DOOR_WIDTH) / 4.0f, z_position, Direction.X, (longEdgeLength - DOOR_WIDTH) / 2.0f);
			GameObject leftDoor = SpawnDoorPrefab (x_position - DOOR_WIDTH / 4.0f, z_position, Direction.X, DOOR_WIDTH / 2.0f);
			GameObject rightDoor = SpawnDoorPrefab (x_position + DOOR_WIDTH / 4.0f, z_position, Direction.X, DOOR_WIDTH / 2.0f);

			leftDoor.GetComponent<DoorControlScript> ().SetOpeningAxis (DoorControlScript.OpenDirection.NEG_X);
			rightDoor.GetComponent<DoorControlScript> ().SetOpeningAxis (DoorControlScript.OpenDirection.POS_X);

			return new GameObject[] { leftDoor, rightDoor };
		} else {
			CreateWall(x_position, z_position - (longEdgeLength + DOOR_WIDTH) / 4.0f, Direction.Z, (longEdgeLength - DOOR_WIDTH) / 2.0f);
			CreateWall(x_position, z_position + (longEdgeLength + DOOR_WIDTH) / 4.0f, Direction.Z, (longEdgeLength - DOOR_WIDTH) / 2.0f);
			GameObject backwardDoor = SpawnDoorPrefab (x_position, z_position - DOOR_WIDTH / 4.0f, Direction.Z, DOOR_WIDTH / 2.0f);
			GameObject forwardDoor = SpawnDoorPrefab (x_position, z_position + DOOR_WIDTH / 4.0f, Direction.Z, DOOR_WIDTH / 2.0f);

			backwardDoor.GetComponent<DoorControlScript> ().SetOpeningAxis (DoorControlScript.OpenDirection.NEG_Z);
			forwardDoor.GetComponent<DoorControlScript> ().SetOpeningAxis (DoorControlScript.OpenDirection.POS_Z);

			return new GameObject[] { backwardDoor, forwardDoor };
		}
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

		/* Performs the union of the sets containing x and y, if x and y are not already in the same set. If x and y are in the same set, nothing happens. */
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

		/* Returns true if x and y are in the same set. */
		public bool InSameSet (T x, T y) {
			return Find (x) == Find (y);
		}

		/* Returns the sets. */
		public List<List<T>> GetSets () {
			return sets;
		}

		/* Returns the number of sets. */ 
		public int NumberOfSets () {
			return sets.Count;
		}

		/* Returns a deep copy of this structure. */
		public UnionFind<T> Copy () {
			UnionFind<T> clone = new UnionFind<T> ();
			foreach (List<T> set in sets) {
				List<T> clonedSet = new List<T> (set);
				clone.sets.Add (clonedSet);
			}
			return clone;
		}
	}
}
