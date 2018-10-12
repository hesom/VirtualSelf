using UnityEngine;


namespace VirtualSelf.GameSystems {


/// <summary>
/// TODO: Fill out this class description: RoomUpdater
/// </summary>
[RequireComponent(typeof(CodesRoomsUi))]
public sealed class RoomUpdater : MonoBehaviour {

    /* ---------- Variables & Properties ---------- */

    private CodesRoomsUi uiScript;


    /* ---------- Methods ---------- */

    private void Start() {

        uiScript = GetComponent<CodesRoomsUi>();
    }

    private void Update() {

        uiScript.UpdateAllPanels();
    }


    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}