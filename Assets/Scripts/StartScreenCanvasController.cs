using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenCanvasController : MonoBehaviour {

	private GameObject rootPrefab;
	private float difficulty = 1.0f;

	private Dropdown difficultyDropdown;

	// Use this for initialization
	void Start () {
		rootPrefab = Resources.Load<GameObject> ("Prefab/Root");
		difficultyDropdown = transform.Find ("DifficultySelect").GetComponent<Dropdown> ();
	}
	
	public void OnStartClicked () {
		Instantiate<GameObject> (rootPrefab).GetComponent<GenerateRoom>().difficultyMultiplier = difficulty;
		Destroy (gameObject);
	}

	public void OnQuitClicked () {
		Application.Quit ();
	}

	public void OnDifficultySelected () {
		if (difficultyDropdown.value == 0) {
			difficulty = 0.75f;
		} else if (difficultyDropdown.value == 1) {
			difficulty = 1.0f;
		} else if (difficultyDropdown.value == 2) {
			difficulty = 1.25f;
		} else if (difficultyDropdown.value == 3) {
			difficulty = 1.5f;
		} else if (difficultyDropdown.value == 4) {
			difficulty = 2.0f;
		} else if (difficultyDropdown.value == 5) {
			difficulty = 3.0f;
		} else {
			throw new UnityException ("unhandled");
		}
	}
}
