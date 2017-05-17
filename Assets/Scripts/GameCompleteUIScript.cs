using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCompleteUIScript : MonoBehaviour {

	private Text messageUI;
	private string message = null;

	// Use this for initialization
	void Start () {
		messageUI = transform.Find ("GameCompleteMessage").GetComponent<Text> ();
		if (message != null) {
			messageUI.text = message;
		}
	}

	public void SetMessage (string m) {
		message = m;
		if (messageUI != null) {
			messageUI.text = message;
		}
	}
	
	public void OnRestartClicked () {
		SceneManager.LoadScene (0);
	}

	public void OnQuitClicked () {
		Application.Quit ();
	}
}
