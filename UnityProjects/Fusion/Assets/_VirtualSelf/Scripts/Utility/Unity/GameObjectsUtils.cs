using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace VirtualSelf.Utility {

    
/// <summary>
/// A collection of static utility methods for working with Unity <see cref="GameObject"/>s in
/// different ways.
/// </summary>
public static class GameObjectsUtils {

    /* ---------- Public Methods ---------- */

    /// <summary>
    /// Returns a list of all child game objects of the given game object. More concretely, this
    /// means all game objects that are part of the Unity Hierarchy of the given game object.<br/>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description>
    /// The game object itself is also included, as the first object in the list.
    /// </description></item>
    /// <item><description>
    /// The hierarchical structure of the game objects is lost - they are all returned "flattened"
    /// into a list.
    /// </description></item>
    /// <item><description>
    /// The order of the game objects in the returned list is undefined, except for the first
    /// element.
    /// </description></item>
    /// </list>
    /// </remarks>
    /// </summary>
    /// <param name="rootObject">
    /// The game object to return all child game objects of. As it is the topmost object of the
    /// return hierarchy, it can be considered the "root object".
    /// </param>
    /// <param name="includeInactiveObjects">
    /// Whether to include game objects that are set to "inactive" within Unity.
    /// </param>
    /// <returns>
    /// A (writeable) list of all child game objects of the given game object, including the game
    /// object itself as its first element.
    /// </returns>
    public static IList<GameObject> CollectAllChildObjectsOf(
            GameObject rootObject,
            bool includeInactiveObjects = true) {

        Transform[] transforms = rootObject.GetComponentsInChildren<Transform>(
                                 includeInactiveObjects);

        IList<GameObject> objects = transforms.Select(trans => (trans.gameObject)).ToList();

        return (objects);
    }
    
    /// <summary>
    /// Print the path of the gameobject in its hierarchy (scene transforms or folders for prefab).
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
    
    /// <summary>
    /// Finds only GameObjects and Components, but no prefabs. Other Unity Object types are omitted by choice here.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T[] FindObjectsOfTypeButNoPrefabs<T>() where T: Object 
    {
        T[] allGameObjects = Resources.FindObjectsOfTypeAll<T>();
        return allGameObjects.Where(x => x is GameObject ? (x as GameObject).scene.name != null : (x as Component)?.gameObject.scene.name != null).ToArray();
    }
}

}