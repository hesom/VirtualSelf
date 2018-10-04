using System.Collections;
using System.Collections.Generic;
using RoboRyanTron.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using VirtualSelf;
using VirtualSelf.GameSystems;

public class LoadFirstScene : MonoBehaviour
{

    public SceneReference startScene;
	public KeycodesList keycodes;

	void Awake() 
	{
		keycodes.Initialize();	
	}

	void Start()
    {
        bool startSceneLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name == startScene.SceneName)
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
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(startScene.SceneName));

	        foreach (var light in FindObjectsOfType<Light>())
	        {
		        LayerUtils.DeactivateLightCullingLayer(light, "Behind Portal");
	        }
        }
    }

	private IEnumerator LoadStartScene()
	{
		var async = SceneManager.LoadSceneAsync(startScene.SceneName, LoadSceneMode.Additive);
		yield return new WaitUntil(() => {return async.isDone;});
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(startScene.SceneName));
	}

	void Update() {
		if(Input.GetKey(KeyCode.Escape)){
			Application.Quit();
		}	
	}

}
