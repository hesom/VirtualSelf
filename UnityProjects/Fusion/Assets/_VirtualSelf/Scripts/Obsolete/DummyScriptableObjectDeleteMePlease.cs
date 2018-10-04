using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// [CreateAssetMenu]
public class DummyScriptableObjectDeleteMePlease : ScriptableObject {

    // list [ enum left/right, monobehavior script, string field, enum replaceOnPortalTraverse/replaceOnSceneLoad ]

    public GameObject Mono;
    
    public void Init()
    {
        // get the left and right hand model
    }

    public void SceneSwitch()
    {
        SceneManager.GetActiveScene();
        // iterate through list, filter scripts with same scene name, do the swap
        Mono.name = "bla";
    }
}
