using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualSelf
{
    
/// <summary>
/// I was trying to avoid duplicate code here, providing two Indicator implementations, one that changes the material
/// and one that changes the color.
/// After 4 painful hours, it seems much like this is impossible. Using subclasses or interfaces in C# is hard enough,
/// but on top of that Unity serialization breaks way too easily.
/// Interfaces cannot provide any implementations, overriding methods is atrocious via both intefaces and subclasses.
/// Lambdas don't work in constructors if they require access to non-static fields.
/// Using reflection with delegates may have worked, but who knows; subclassing would be pretty convoluted anyways. 
/// Even with the final "solution", which compiled, ignoring the fact that a mandadory method override did not work,
/// ignoring that serialization throws exceptions at you, there is then no way to actually use the superclass,
/// because generics in C# also suck.
/// Even ignoring that it is not possible to assign the script in the inspector, it is also not possible to access
/// the generic component via a find method; the only way would be to check for all possible subclasses by hand.
/// </summary>
/// <typeparam name="T"></typeparam>

public interface IIndicator<T> {
    
//    States State { get; set; } 

    T GetVisualForState(IndicatorFail<T>.States state);
//    void SetState(States s);
//    void SetValidThenIndetermined();
//    void SetIndermined();
    
}
    
//public delegate void BiConsumer<T,U>(T arg1, U arg2);

public class IndicatorFail<T> : MonoBehaviour, IIndicator<T>
{
    public enum States {
        Indetermined, Valid, Invalid
    }
    
//    public Indicator(Action<Renderer,T> visualSetter, Func<States,T> visualProvider)
    public IndicatorFail(Action<Renderer,T> visualSetter)
    {
//        Func<object,Action<Material>> a = (aa) => GetComponent<Renderer>().material;
//        Renderer r = GetComponent<Renderer>();
//        r.material.color
//        Delegate.CreateDelegate(typeof(T), r, typeof(Renderer).GetMethod("material"));
        _visualSetter = visualSetter;
//        _visualProvider = visualProvider;
    }

    public T Indetermined, Valid, Invalid;
    public States State
    {
        get { return _state; }
        set { _state = value;
//            _visualSetter.Invoke(GetComponent<Renderer>(), _visualProvider.Invoke(value));
            _visualSetter.Invoke(GetComponent<Renderer>(), GetVisualForState(value));
        }
    }

    private readonly Action<Renderer,T> _visualSetter;
//    private readonly Func<States,T> _visualProvider;
    private States _state;
    
    public void SetValidThenIndetermined()
    {
        State = States.Valid;
        CancelInvoke("SetIndermined");
        Invoke("SetIndermined", 1f);
    }
    
    void Start ()
    {
        SetIndermined();
    }
    
    private void SetIndermined()
    {
        State = States.Indetermined;
    }

    public virtual T GetVisualForState(States state)
    {
        throw new NotImplementedException("subclass of Indicator needs to implement this");
    }
}
    
}
