using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VirtualSelf;

public class LoadFirstScene : MonoBehaviour
{

    public string startScene;

    void Start()
    {
        bool startSceneLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name == startScene)
            {
                startSceneLoaded = true;
            }
        }

        if (!startSceneLoaded)
        {
            StartCoroutine(LoadStartScene());
			startSceneLoaded = true;
        }
        else
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(startScene));

	        foreach (var light in FindObjectsOfType<Light>())
	        {
		        LayerUtils.DeactivateLightCullingLayer(light, "Behind Portal");
	        }
        }
    }

	private IEnumerator LoadStartScene()
	{
		var async = SceneManager.LoadSceneAsync(startScene, LoadSceneMode.Additive);
		yield return new WaitUntil(() => {return async.isDone;});
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(startScene));
	}

	void Update() {
		if(Input.GetKey(KeyCode.Escape)){
			Application.Quit();
		}	
	}

}
