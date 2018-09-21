using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;

namespace VirtualSelf
{

public class HandProxy : HandModelBase
{
    public enum HandType {Capsule, Rigged}
    
    public Chirality Chirality;
    public HandType Type;
    
    private HandModelBase _source;

    void Awake()
    {
        HandModelBase[] handmodels = FindObjectsOfType<HandModelBase>();
        foreach (var h in handmodels)
        {
            if (IsCompatible(h))
            {
                _source = h;
                break;
            }
        }
        if (_source == null) throw new InvalidOperationException("did not find a matching hand model, was the master scene loaded?");
    }

    // check if the given hand meets the proxy requirements
    private bool IsCompatible(HandModelBase h)
    {
        if (h == this) return false;
        if (h is HandProxy) return false; // no proxies
        if (h.Handedness != Chirality) return false; // left/right match
        if (h is CapsuleHand && Type == HandType.Capsule) return true;
        if (h is RiggedHand && Type == HandType.Rigged) return true;

        return false;
    }

    public override Chirality Handedness
    {
        get { return _source.Handedness; }
        set { _source.Handedness = value; }
    }
    public override ModelType HandModelType => _source.HandModelType;

    public override void UpdateHand()
    {
        _source.UpdateHand();
    }

    public override Hand GetLeapHand()
    {
        return _source.GetLeapHand();
    }

    public override void SetLeapHand(Hand hand)
    {
        _source.SetLeapHand(hand);
    }
}

}