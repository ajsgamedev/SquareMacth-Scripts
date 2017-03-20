using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseAndLoadScene : MonoBehaviour {

	public string levelToLoad;
	public float delay = 2.0f;

	// use invoke to wait for a delay then call LoadLevel
	void Update () {
		Invoke("LoadLevel",delay);
	}

	// load the specified level
	void LoadLevel() {
		SceneManager.LoadScene(levelToLoad);
	}
}
