using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateRoom : MonoBehaviour {

	private GameObject playerPrefab;
	private GameObject groundPrefab;
	private GameObject wallPrefab;
	private GameObject fogPrefab;
	private GameObject singleDoorPrefab;

	private GameObject[] enemyPrefabs;
	private GameObject[] obstaclePrefabs;
	private GameObject[] itemPrefabs;
	private GameObject[] weaponPrefabs;
	private GameObject bandagePrefab;

	private Floor floor;
	private GameObject player;

	private bool playerSpawned;

	// Use this for initialization
	void Start () {
		playerSpawned = false;
		playerPrefab = Resources.Load<GameObject> ("Prefab/Player");
		groundPrefab = Resources.Load<GameObject> ("Prefab/Ground");
		wallPrefab = Resources.Load<GameObject> ("Prefab/Wall");
		singleDoorPrefab = Resources.Load<GameObject> ("Prefab/SingleDoor");
		fogPrefab = Resources.Load<GameObject> ("Prefab/Fog");

		enemyPrefabs = Resources.LoadAll<GameObject> ("Prefab/Enemies");
		obstaclePrefabs = Resources.LoadAll<GameObject> ("Prefab/Obstacles");
		itemPrefabs = Resources.LoadAll<GameObject> ("Prefab/Items");
		weaponPrefabs = Resources.LoadAll<GameObject> ("Prefab/Weapons/PlayerWeapons");
		bandagePrefab = Resources.Load<GameObject> ("Prefab/Bandage");

		floor = new Floor(this, 10, 10);
		floor.generateGameObjects ();
	}

	/*void Update () {		
		if (playerSpawned) {
			Room currentRoom = floor.GetRoomContaining (player.transform.position);
			currentRoom.ClearFog ();

			if (currentRoom.CheckIfCleared ()) {
				currentRoom.OpenAllDoors ();
				currentRoom.SpawnRewards ();
			} else {
				/* Spawn enemies/treasure as soon as the doors close. *//*
				if (currentRoom.AllDoorsClosed ()) {
					currentRoom.SpawnEnemies ();
				} else {
					currentRoom.CloseAllDoors ();
				}
			}
		}

		// Developer-mode cheats. Removes all fog.
		if (Input.GetKeyDown (KeyCode.C)) {
			foreach (Room r in floor.cellsToRooms.Values) {
				r.ClearFog ();
			}
		}
	}*/
		
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
		public enum Type {
			STARTING, HOSTILES, TREASURE, EMPTY
		}

		private Floor enclosingInstance;

		// The grid cells of the floor that comprise this room
		public List<Cell> cells; 

		// What type of room this is
		public Type roomType;

		// All the doors leading in and out of this room. Note: 
		// individual doors will appear in TWO different rooms
		private List<GameObject> doors;
		private List<GameObject> fog; 

		private List<GameObject> enemiesToSpawn;
		private List<Vector3> spawnPositions;

		private List<GameObject> aliveEnemies;
		private bool cleared;

		private List<GameObject> rewards;
		private List<Vector3> rewardSpawnPositions;

		public Room (Floor enclosingInstance, List<Cell> cells, Room.Type roomType) {
			this.enclosingInstance = enclosingInstance;
			this.cells = cells;
			this.roomType = roomType;
			this.doors = new List<GameObject>();
			this.fog = new List<GameObject>();
			this.enemiesToSpawn = new List<GameObject>();
			this.spawnPositions = new List<Vector3>();
			this.aliveEnemies = new List<GameObject> ();
			this.cleared = false;

			this.rewards = new List<GameObject> ();
			this.rewardSpawnPositions = new List <Vector3> ();
		}

		public void AddDoor (GameObject door) {
			this.doors.Add (door);
		}

		public void AddFogObject (GameObject fog) {
			this.fog.Add(fog);
		}

		public void AddEnemyToSpawn (GameObject enemy, Vector3 spawnPosition) {
			enemiesToSpawn.Add (enemy);
			spawnPositions.Add (spawnPosition);
		}

		public void AddRewardToSpawn (GameObject reward, Vector3 spawnPosition) {
			rewards.Add (reward);
			rewardSpawnPositions.Add (spawnPosition);
		}

		public void OpenAllDoors () {
			foreach (GameObject door in doors) {
				door.GetComponent<DoorControlScript> ().OpenDoor ();
			}
		}

		public void CloseAllDoors () {
			foreach (GameObject door in doors) {
				door.GetComponent<DoorControlScript> ().CloseDoor ();
			}
		}

		public bool AllDoorsClosed () {
			foreach (GameObject door in doors) {
				if (door.GetComponent<DoorControlScript> ().GetState () != DoorControlScript.DoorState.CLOSED) {
					return false;
				}
			}
			return true;
		}

		public void SpawnEnemies () {
			for (int i = 0; i < enemiesToSpawn.Count; i++) {
				if (CanSpawnObjectAt (enemiesToSpawn [i], spawnPositions [i])) {
					GameObject newEnemy = Instantiate<GameObject> (enemiesToSpawn [i], spawnPositions [i], Quaternion.identity);

					Enemy newEnemyScript = newEnemy.GetComponent<Enemy> ();
					if (newEnemyScript != null) {
						ApplyStrengthMultiplier (newEnemyScript, 1.0f + 0.02f * enclosingInstance.numRoomsCleared);
						aliveEnemies.Add (newEnemy);
					}
				}
			}
			enemiesToSpawn.Clear ();
			spawnPositions.Clear ();
		}

		public void SpawnRewards () {
			for (int i = 0; i < rewards.Count; i++) {
				if (CanSpawnObjectAt (rewards [i], rewardSpawnPositions [i])) {
					Instantiate<GameObject> (rewards [i], rewardSpawnPositions [i], Quaternion.identity);
				}
			}
			rewards.Clear ();
			rewardSpawnPositions.Clear ();
		}

		public bool CheckIfCleared () {
			// Once the room has been cleared, it will always be cleared.
			if (cleared) {
				return true;
			}
				
			if (roomType == Type.HOSTILES) {
				aliveEnemies.RemoveAll (IsNull);
				cleared = aliveEnemies.Count == 0 && enemiesToSpawn.Count == 0;
			} else if (roomType == Type.STARTING || roomType == Type.EMPTY) {
				cleared = true;
			} else if (roomType == Type.TREASURE) {
				cleared = (enemiesToSpawn.Count == 0);
			}
				
			if (cleared) {
				// This code here will only run when the room is first cleared.
				if (roomType != Type.STARTING) {
					enclosingInstance.IncRoomsCleared ();
				}
			}

			return cleared;
		}

		public void ClearFog () {
			foreach (GameObject f in fog) {
				Destroy (f);
			}
			fog.Clear ();
		}

		private static bool IsNull (Object o) {
			return o == null;
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

		// The number of rooms cleared on this floor.
		public int numRoomsCleared;

		// UnionFind structure where each set is all cells that make up one room.
		private UnionFind<Cell> rooms;
		// List<Cells> in a room --> Room object for that room
		public Dictionary<List<Cell>, Room> cellsToRooms;

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

			List<Room.Type> roomTypes = new List<Room.Type>();
			// One starting room.
			roomTypes.Add(Room.Type.STARTING);

			// ~ 10% will be empty, including the starting room.
			while (roomTypes.Count < rooms.NumberOfSets() / 10) {
				roomTypes.Add(Room.Type.EMPTY);
			}

			// ~25% of the rooms will be treasure rooms.
			while (roomTypes.Count < rooms.NumberOfSets() / 10 + rooms.NumberOfSets() / 4) {
				roomTypes.Add(Room.Type.TREASURE);
			}

			// Rest are hostile rooms
			while (roomTypes.Count < rooms.NumberOfSets()) {
				roomTypes.Add(Room.Type.HOSTILES);
			}

			foreach (List<Cell> cellList in rooms.GetSets()) {
				// Room types are doled out at random.
				int rand_index = Random.Range(0, roomTypes.Count);
				cellsToRooms.Add(cellList, new Room(this, cellList, roomTypes[rand_index]));
				roomTypes.RemoveAt(rand_index);
			}

			// Probability of adding an unncessary door (a door that connects two sections that are already connected).
			float probUnnecessaryDoor = 0.3125f; 
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

					Room r;
					GameObject fogObject = enclosingInstance.CreateFog (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, cumulative_delta_zs [j] + delta_zs [j] / 2.0f, delta_xs [i], delta_zs [j]);
					cellsToRooms.TryGetValue(rooms.Find(cells [i, j]), out r);
					r.AddFogObject (fogObject);

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

					int obstacles_to_spawn = 0; 
					int enemies_to_spawn = 0;
					int items_to_spawn = 0;
					int weapons_to_spawn = 0;
					int bandage_rewards_to_spawn = 0;

					if (r.roomType == Room.Type.HOSTILES) {
						obstacles_to_spawn = Random.Range (1, 5);
						enemies_to_spawn = Random.Range (2, 4);
						bandage_rewards_to_spawn = Random.Range (0, 4) - 2; // 1/4 chance for bandage rewards (in each cell of the room).
					} else if (r.roomType == Room.Type.STARTING) {
						if (!enclosingInstance.playerSpawned) {
							enclosingInstance.player = Instantiate<GameObject> (enclosingInstance.playerPrefab, new Vector3 (-250, .5f, -260), enclosingInstance.playerPrefab.transform.rotation);//new Vector3 (cumulative_delta_xs [i] + delta_xs [i] / 2.0f, enclosingInstance.playerPrefab.transform.position.y, cumulative_delta_zs [j] + delta_zs [j] / 2.0f), enclosingInstance.playerPrefab.transform.rotation);
							enclosingInstance.playerSpawned = true;
						}
						obstacles_to_spawn = 3;
					} else if (r.roomType == Room.Type.TREASURE) {
						obstacles_to_spawn = Random.Range (1, 5);
						// 1/3 for no weapon, 2/3 for one weapon
						weapons_to_spawn = Random.Range(0, 3);
						if (weapons_to_spawn == 2) {
							weapons_to_spawn = 1;
						}

						// 1/3 for no item, 1/2 for one item, 1/6 for two items
						items_to_spawn = Random.Range (0, 3); 
						items_to_spawn -= Random.Range (0, items_to_spawn); // Note; Random.Range(0,0) always returns 0.

						// 1/2 chance for a bandage
						bandage_rewards_to_spawn = Random.Range(0, 2);
					} else if (r.roomType == Room.Type.EMPTY) {
						obstacles_to_spawn = Random.Range (3, 11);
					}

					const int GIVE_UP_THRESHOLD = 100; // Avoid infinite loops if nothing can be spawned.

					int spawn_attempts = 0;
					while (obstacles_to_spawn > 0 && spawn_attempts < GIVE_UP_THRESHOLD) {
						GameObject obstacleToSpawn = enclosingInstance.obstaclePrefabs [Random.Range (0, enclosingInstance.obstaclePrefabs.Length)];

						Vector3 spawnPosition = new Vector3 (cumulative_delta_xs [i] + Random.Range (0.0f, delta_xs [i]), obstacleToSpawn.transform.position.y, cumulative_delta_zs [j] + Random.Range (0.0f, delta_zs [j]));
						Quaternion spawnOrientation = Quaternion.Euler (new Vector3 (0.0f, Random.Range (0.0f, 360.0f), 0.0f));

						if (CanSpawnObjectAt(obstacleToSpawn, spawnPosition, spawnOrientation)) {
							Instantiate<GameObject> (obstacleToSpawn, spawnPosition, spawnOrientation, enclosingInstance.transform);
							obstacles_to_spawn--;
						}
						spawn_attempts++;
					}

					spawn_attempts = 0;
					while (items_to_spawn > 0 && spawn_attempts < GIVE_UP_THRESHOLD) {
						GameObject itemToSpawn = enclosingInstance.itemPrefabs [Random.Range (0, enclosingInstance.itemPrefabs.Length)];

						Vector3 spawnPosition = new Vector3 (cumulative_delta_xs [i] + Random.Range (0.0f, delta_xs [i]), itemToSpawn.transform.position.y, cumulative_delta_zs [j] + Random.Range (0.0f, delta_zs [j]));

						if (CanSpawnObjectAt (itemToSpawn, spawnPosition)) {
							r.AddEnemyToSpawn (itemToSpawn, spawnPosition);
							items_to_spawn--;
						}
						spawn_attempts++;
					}

					spawn_attempts = 0;
					while (weapons_to_spawn > 0 && spawn_attempts < GIVE_UP_THRESHOLD) {
						GameObject itemToSpawn = enclosingInstance.weaponPrefabs [Random.Range (0, enclosingInstance.weaponPrefabs.Length)];

						Vector3 spawnPosition = new Vector3 (cumulative_delta_xs [i] + Random.Range (0.0f, delta_xs [i]), itemToSpawn.transform.position.y, cumulative_delta_zs [j] + Random.Range (0.0f, delta_zs [j]));

						if (CanSpawnObjectAt (itemToSpawn, spawnPosition)) {
							r.AddEnemyToSpawn (itemToSpawn, spawnPosition);
							weapons_to_spawn--;
						}
						spawn_attempts++;
					}

					spawn_attempts = 0;
					while (enemies_to_spawn > 0 && spawn_attempts < GIVE_UP_THRESHOLD) {
						GameObject enemyToSpawn = enclosingInstance.enemyPrefabs [Random.Range (0, enclosingInstance.enemyPrefabs.Length)];

						Vector3 spawnPosition = new Vector3 (cumulative_delta_xs [i] + Random.Range(0.0f, delta_xs[i]), enemyToSpawn.transform.position.y, cumulative_delta_zs [j] + Random.Range(0.0f, delta_zs[j]));

						if (CanSpawnObjectAt(enemyToSpawn, spawnPosition)) {
							r.AddEnemyToSpawn (enemyToSpawn, spawnPosition);
							enemies_to_spawn--;
						}
						spawn_attempts++;
					}

					spawn_attempts = 0;
					while (bandage_rewards_to_spawn > 0 && spawn_attempts < GIVE_UP_THRESHOLD) {
						Vector3 spawnPosition = new Vector3 (cumulative_delta_xs [i] + Random.Range(0.0f, delta_xs[i]), enclosingInstance.bandagePrefab.transform.position.y, cumulative_delta_zs [j] + Random.Range(0.0f, delta_zs[j]));
						if (CanSpawnObjectAt (enclosingInstance.bandagePrefab, spawnPosition)) {
							r.AddRewardToSpawn (enclosingInstance.bandagePrefab, spawnPosition);
							bandage_rewards_to_spawn--;
						}

						spawn_attempts++;
					}
				}
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

		public void IncRoomsCleared () {
			enclosingInstance.player.GetComponent<MainCharacterController> ().IncRoomsCleared ();
			numRoomsCleared++;
		}

		public Room GetRoomContaining(Vector3 position) {
			Room room;
			cellsToRooms.TryGetValue (rooms.Find(GetCellContaining (position.x, position.z)), out room);
			return room;
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

	/* Creates a fog object (over a cell). Returns the game object it creates. */
	private GameObject CreateFog (float x_position, float z_position, float x_size, float z_size) {
		// The + 0.01f to the y-position prevents some weird graphical glitch.
		GameObject fog = Instantiate<GameObject> (fogPrefab, new Vector3 (x_position, wallPrefab.transform.localScale.y + 0.01f, z_position), Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f)), transform);		
		fog.transform.localScale = new Vector3 (x_size, z_size, 1.0f);
		return fog;
	}

	private static bool CanSpawnObjectAt (GameObject spawn, Vector3 spawnPosition) {
		return CanSpawnObjectAt (spawn, spawnPosition, Quaternion.identity);
	}

	private static bool CanSpawnObjectAt (GameObject spawn, Vector3 spawnPosition, Quaternion orientation) {
		// Must check each type of collider individually.
		// I am using GetComponentInChildren because some of the MMMM assests have colliders in seperate elements.

		const int LAYER_MASK = ~(1 << 8); // Ignores the ground and the fog objects. They are put in layer 8.
		const float BUFFER_WIDTH = 0.5f; // Buffer between the object and any other objects.

		CapsuleCollider capsule = spawn.GetComponentInChildren<CapsuleCollider> ();
		if (capsule != null) {
			Vector3 start, end;
			if (capsule.direction == 0) {
				start = spawnPosition + (capsule.height - capsule.radius) * Vector3.left;
				end = spawnPosition + (capsule.height - capsule.radius) * Vector3.right;
			} else if (capsule.direction == 1) {
				start = spawnPosition + (capsule.height - capsule.radius) * Vector3.down;
				end = spawnPosition + (capsule.height - capsule.radius) * Vector3.up;
			} else if (capsule.direction == 2) {
				start = spawnPosition + (capsule.height - capsule.radius) * Vector3.back;
				end = spawnPosition + (capsule.height - capsule.radius) * Vector3.forward;
			} else {
				throw new UnityException ("Unexepted capsule direction: " + capsule.direction);
			}
			return !Physics.CheckCapsule (start, end, capsule.radius + BUFFER_WIDTH, LAYER_MASK);
		}

		BoxCollider box = spawn.GetComponentInChildren<BoxCollider> ();
		if (box != null) {
			return !Physics.CheckBox (spawnPosition + box.center, box.size / 2.0f + new Vector3(BUFFER_WIDTH, BUFFER_WIDTH, BUFFER_WIDTH), orientation, LAYER_MASK);
		}

		SphereCollider sphere = spawn.GetComponentInChildren<SphereCollider> ();
		if (sphere != null) {
			return !Physics.CheckSphere (spawnPosition + sphere.center, sphere.radius + BUFFER_WIDTH);
		}

		Debug.Log ("Warning: CanSpawnObject() failed to check the collider of an object");
		return true;
	}

	private static void ApplyStrengthMultiplier (Enemy e, float multiplier) {
		e.bulletDamage *= multiplier;
		e.bulletDelay *= multiplier;
		e.contactDamage *= multiplier;
		e.MAX_HEALTH *= multiplier;
		e.knockBackResistance *= multiplier;
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
