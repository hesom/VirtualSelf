using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _7Segment : MonoBehaviour
{
    public Material transparent;
    public Material opaque;
    public char defaultChar = '8';

    // interpretation: top to bottom, then left to right
    private bool[] blocked;
    private MeshRenderer[] segments = new MeshRenderer[7];

    // Use this for initialization
    void Start ()
    {
        for (int i = 0; i < 7; i++) segments[i] = transform.GetChild(i).GetComponent<MeshRenderer>();
        SetCharacter(defaultChar);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetCharacter(char c)
    {
        switch (c)
        {
            case '0':
                blocked = new bool[] {true, true, true, false, true, true, true};
                break;
            case '1':
                blocked = new bool[] {false, false, true, false, false, true, false};
                break;
            case '2':
                blocked = new bool[] {true, false, true, true, true, false, true};
                break;
            case '3':
                blocked = new bool[] {true, false, true, true, false, true, true};
                break;
            case '4':
                blocked = new bool[] {false, true, true, true, false, true, false};
                break;
            case '5':
                blocked = new bool[] {true, true, false, true, false, true, true};
                break;
            case '6':
                blocked = new bool[] {true, true, false, true, true, true, true};
                break;
            case '7':
                blocked = new bool[] {true, false, true, false, false, true, false};
                break;
            case '8':
                blocked = new bool[] {true, true, true, true, true, true, true};
                break;
            case '9':
                blocked = new bool[] {true, true, true, true, false, true, true};
                break;
            case ' ':
                blocked = new bool[] {false, false, false, false, false, false, false};
                break;
            default:
                Debug.LogWarning("7Segment is missing implementation for character "+c);
                break;
        }

        for (int i = 0; i < 7; i++)
        {
            segments[i]
            //transform.GetChild(i).GetComponent<MeshRenderer>()
                    .material = 
                blocked[i] ? 
                    opaque : 
                    transparent;
        }
    }
}
