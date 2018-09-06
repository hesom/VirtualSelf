using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Key
{
    K1, K2, K3, K4, K5, K6, K7, K8, K9, K0, KStar, KHash
}

[Serializable]
public class StringEvent : UnityEvent<string> { }

public class InputLogic : MonoBehaviour
{
    public StringEvent OnSequenceComplete;

    private _7Segment[] segments;
    private int currentSlot;
    private char[] currentSequence;
    private bool waitingForClear;

	// Use this for initialization
	void Start () {
        segments = new _7Segment[transform.childCount];
	    for (int i = 0; i < transform.childCount; i++) segments[i] = transform.GetChild(i).GetComponent<_7Segment>();
        currentSequence = new char[segments.Length];

	    StartClear();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartClear()
    {
        waitingForClear = true;
        StartCoroutine(Clear());
    }

    IEnumerator Clear()
    {
        StopCoroutine(Clear());

        yield return new WaitForSeconds(1);

        currentSlot = 0;
        for (int i = currentSequence.Length-1; i >=0 ; i--)
        {
            yield return new WaitForSeconds(0.02f);
            currentSequence[i] = ' ';
            segments[i].SetCharacter(' ');
        }

        waitingForClear = false;
    }

    public string GetSequence()
    {
        return new string(currentSequence);
    }

    //[EnumAction(typeof(Key))]
    public void PressKeyWorkaround(int arg)
    {
        PressKey((Key) arg);
    }

    // not supported by unity inspector
    public void PressKey(Key k)
    {
        switch (k)
        {
            case Key.K0:
                PressChar('0');
                break;
            case Key.K1:
                PressChar('1');
                break;
            case Key.K2:
                PressChar('2');
                break;
            case Key.K3:
                PressChar('3');
                break;
            case Key.K4:
                PressChar('4');
                break;
            case Key.K5:
                PressChar('5');
                break;
            case Key.K6:
                PressChar('6');
                break;
            case Key.K7:
                PressChar('7');
                break;
            case Key.K8:
                PressChar('8');
                break;
            case Key.K9:
                PressChar('9');
                break;
            case Key.KStar:
                PressChar('*');
                break;
            case Key.KHash:
                PressChar('#');
                break;
        }
    }

    public void PressChar(char c)
    {
        if (waitingForClear)
        {
            return;
        }

        switch (c)
        {
            case '0':
                for (int i=1;i<=currentSlot;i++) segments[i].SetCharacter(currentSequence[currentSlot-i]);
                    segments[0].SetCharacter(c);
                    currentSequence[currentSlot] = c;
                    break;
            case '1':
                for (int i=1;i<=currentSlot;i++) segments[i].SetCharacter(currentSequence[currentSlot-i]);
                    segments[0].SetCharacter(c);
                    currentSequence[currentSlot] = c;
                    break;
            case '2':
                for (int i=1;i<=currentSlot;i++) segments[i].SetCharacter(currentSequence[currentSlot-i]);
                    segments[0].SetCharacter(c);
                    currentSequence[currentSlot] = c;
                    break;
            case '3':
                for (int i=1;i<=currentSlot;i++) segments[i].SetCharacter(currentSequence[currentSlot-i]);
                    segments[0].SetCharacter(c);
                    currentSequence[currentSlot] = c;
                    break;
            case '4':
                for (int i=1;i<=currentSlot;i++) segments[i].SetCharacter(currentSequence[currentSlot-i]);
                    segments[0].SetCharacter(c);
                    currentSequence[currentSlot] = c;
                    break;
            case '5':
                for (int i=1;i<=currentSlot;i++) segments[i].SetCharacter(currentSequence[currentSlot-i]);
                    segments[0].SetCharacter(c);
                    currentSequence[currentSlot] = c;
                    break;
            case '6':
                for (int i=1;i<=currentSlot;i++) segments[i].SetCharacter(currentSequence[currentSlot-i]);
                    segments[0].SetCharacter(c);
                    currentSequence[currentSlot] = c;
                    break;
            case '7':
                for (int i=1;i<=currentSlot;i++) segments[i].SetCharacter(currentSequence[currentSlot-i]);
                    segments[0].SetCharacter(c);
                    currentSequence[currentSlot] = c;
                    break;
            case '8':
                for (int i=1;i<=currentSlot;i++) segments[i].SetCharacter(currentSequence[currentSlot-i]);
                    segments[0].SetCharacter(c);
                    currentSequence[currentSlot] = c;
                break;
            case '9':
                for (int i=1;i<=currentSlot;i++) segments[i].SetCharacter(currentSequence[currentSlot-i]);
                segments[0].SetCharacter(c);
                currentSequence[currentSlot] = c;
                break;
            case '*':
                // TODO add functionality
                break;
            case '#':
                // TODO add functionality
                break;
            default:
                Debug.LogWarning("Unexpected PressChar value "+c);
                break;
        }

        currentSlot++;

        if (currentSlot >= segments.Length)
        {
            OnSequenceComplete.Invoke(GetSequence());
            StartClear();
        }
    }
}
