using System;
using UnityEngine.Events;


namespace VirtualSelf.Utility.UnityEvents {


/// <summary>
/// A <see cref="UnityEvent"/> class with one argument of type <c>bool</c>. This UE can be used in
/// Unity Editors (e.g. Inspectors).
/// </summary>
[Serializable]
public sealed class BoolUE : UnityEvent<bool> { }
    
/// <summary>
/// A <see cref="UnityEvent"/> class with one argument of type <c>int</c>. This UE can be used in
/// Unity Editors (e.g. Inspectors).
/// </summary>
[Serializable]
public sealed class IntUE : UnityEvent<int> { }
    
/// <summary>
/// A <see cref="UnityEvent"/> class with one argument of type <c>float</c>. This UE can be used in
/// Unity Editors (e.g. Inspectors).
/// </summary>
[Serializable]
public sealed class FloatUE : UnityEvent<float> { }

/// <summary>
/// A <see cref="UnityEvent"/> class with one argument of type <c>string</c>. This UE can be used in
/// Unity Editors (e.g. Inspectors).
/// </summary>
[Serializable]
public sealed class StringUE : UnityEvent<string> { }
    
/// <summary>
/// A <see cref="UnityEvent"/> class with one argument of type <see cref="UnityEngine.Object"/>.
/// This UE can be used in Unity Editors (e.g. Inspectors).
/// </summary>
[Serializable]
public sealed class ObjectUE : UnityEvent<UnityEngine.Object> { }
}