using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadCurrentSceneOnKey : MonoBehaviour {

    public KeyCode key;

	private bool whichFrame;
	private string name;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(key))
        {
	        Scene activeScene = SceneManager.GetActiveScene();
	        name = activeScene.name;
	        SceneManager.UnloadSceneAsync(activeScene);
            SceneManager.LoadScene(activeScene.name, LoadSceneMode.Additive);  
	        whichFrame = true;
        }
        else {
	        if (whichFrame) {
		        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
		        whichFrame = false;
	        }
        }
	}
}
