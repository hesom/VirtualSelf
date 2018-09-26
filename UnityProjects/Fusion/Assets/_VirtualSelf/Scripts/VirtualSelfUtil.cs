using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace VirtualSelf
{

public static class VirtualSelfUtil 
{
    public static MemberInfo GetFieldOrProperty(Type t, string name)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase;
        FieldInfo f = t.GetField(name, flags);
        if (f != null) return f;
        PropertyInfo p = t.GetProperty(name, flags);
        if (p != null) return p;

        throw new EntryPointNotFoundException("no field or property "+name+" in "+t);
    }

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
    public static T[] FindObjectsOfTypeButNoPrefabs<T>() where T: UnityEngine.Object 
    {
        T[] allGameObjects = Resources.FindObjectsOfTypeAll<T>();
        return allGameObjects.Where(x => x is GameObject ? (x as GameObject).scene.name != null : (x as Component)?.gameObject.scene.name != null).ToArray();
    }
}
    
}
