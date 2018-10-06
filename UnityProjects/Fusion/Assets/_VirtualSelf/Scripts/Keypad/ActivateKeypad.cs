using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VirtualSelf.GameSystems;


public class ActivateKeypad : MonoBehaviour {

	public Keycode Code;
	
	void Start() {

		Scene masterScene = SceneManager.GetSceneByName("MasterScene");

		if (masterScene.IsValid()) {

			masterScene.GetRootGameObjects().ToList().ForEach(elem => {
				
				if (elem.CompareTag("Keypad")) { elem.SetActive(true); }
			});
		}

		Code.IsDiscovered = true;
	}

}
