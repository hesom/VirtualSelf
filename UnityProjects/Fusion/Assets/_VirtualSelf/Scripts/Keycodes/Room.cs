using RoboRyanTron.SceneReference;
using UnityEngine;

namespace VirtualSelf.GameSystems {

/// <summary>
/// TODO: Fill out this class description: Room
/// </summary>
[CreateAssetMenu(
    fileName = "Room",
    menuName = "Keycodes/Room"
)]
public sealed class Room : ScriptableObject {

    /* ---------- Variables & Properties ---------- */

    public const string fieldNameRoomName = nameof(roomName);
    public const string fieldNameDescription = nameof(description);
    public const string fieldNameScene = nameof(scene);
    
    public string Name => roomName;

    public string Description => description;

    public bool IsDiscovered;

    [SerializeField]
    private string roomName;
    
    [SerializeField]
    private SceneReference scene;

    [SerializeField]
    private string description;


    /* ---------- Constructors ---------- */






    /* ---------- Methods ---------- */






    /* ---------- Overrides ---------- */

    private void OnValidate() {

        /* If the name for this Room is still empty, we will set it to the name of the scene - this
         * is better than just leaving it empty. */
        
        if (roomName == "") {

            Debug.Log("Room name is empty");
            
            if (scene != null) {
                
                roomName = scene.Scene.name;
            }
        }
    }


    /* ---------- Inner Classes ---------- */






}

}