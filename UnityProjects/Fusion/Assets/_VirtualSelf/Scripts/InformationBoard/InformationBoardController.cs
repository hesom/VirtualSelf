using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VirtualSelf.CubeScripts;
using VirtualSelf.Utility;
using Object = UnityEngine.Object;


namespace VirtualSelf.GameSystems {


/// <summary>
/// TODO: Fill out this class description: InformationBoardController
/// </summary>
public sealed class InformationBoardController : MonoBehaviour {
     
    /* ---------- Enumerations ---------- */

    public enum NotifyState {

        Off,
        Notifying,
        Emergency
    }

    public enum Direction {

        Left,
        Right
    }
    
    
    /* ---------- Variables & Properties ---------- */

    private static float MoveOffset = 30.0f;

    private const string MessageKeycodeDiscoveredFistPart =
        "You have discovered a new keyccode! The keycode is <b>";
    private const string MessageKeycodeDiscoveredSecondPart =
        "</b>.\n\nRemember that you can also check all the keycodes you have discovered, and " + 
        "their associated rooms, in the <b>Hub Room</b> or on your <b>Keycodes List</b> screen.";
    
    public LampAnimator LeftLight;
    public LampAnimator RightLight;

    public Canvas TextCanvas;
    public ScrollRect TextScrollRect;
    public RectTransform TextContentTransform;
    public Text TextObject;

    public Scrollbar ScrollbarObject;

    public BoardMessageChannel MessageChannel;

    public KeycodesList KeycodesList;

    // TODO: Make private after testing
    public NotifyState notifyState = NotifyState.Off;
    
    private List<Message> messages = new List<Message>();


    /* ---------- Methods ---------- */

    private void Start() {

        MessageChannel.MessageSent += OnMessageReceived;
        KeycodesList.OnKeycodeStateChanged.AddListener(OnKeycodeDiscovered);
    }

    private void Update() {

        if (notifyState == NotifyState.Off) {
            
            // TODO: Lamps
        }

        if (Input.GetKeyDown(KeyCode.M)) {
                      
            Debug.Log("Moving out");
            StartCoroutine(SwitchToMessage(new Message("This worked out I guess")));
        }
        if (Input.GetKeyDown(KeyCode.N)) {
                    
            ScrollbarObject.gameObject.SetActive(false);
        }
        
    }

    private void AddMessage(Message message) {
        
        messages.Add(message);

        if (message.NotifyPlayer) { notifyState = NotifyState.Notifying; }
        
        
    }

    private IEnumerator MoveText(Direction direction, float timeInSeconds, float distance) {

        // Debug.Log("Position at coroutine beginning: " + TextContentObject.transform.localPosition);

        float stepLength = (distance / (timeInSeconds / 0.01f));
        
        if (direction == Direction.Right) {
            
            for (float f = 0.0f; f <= distance; f += stepLength) {
           
                TextContentTransform.anchoredPosition = 
                    new Vector2(
                        TextContentTransform.anchoredPosition.x + stepLength,
                        TextContentTransform.anchoredPosition.y
                    );
            
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (direction == Direction.Left) {
            
            for (float f = distance; f >= 0.0f; f -= stepLength) {
           
                TextContentTransform.anchoredPosition = 
                    new Vector2(
                        TextContentTransform.anchoredPosition.x - stepLength,
                        TextContentTransform.anchoredPosition.y
                    );
            
                yield return new WaitForSeconds(0.01f);
            }
        }
       
    }

    private IEnumerator SwitchToMessage(Message message) {
        
        ScrollbarObject.gameObject.SetActive(false);
       
        yield return StartCoroutine(MoveText(Direction.Right, 0.5f, TextCanvas.pixelRect.width + MoveOffset));

        Debug.Log("Move out done");
        
        // TextObject.text = "";
        
        TextObject.text = message.Text;
        
        // Debug.Log("Position before teleport: " + TextContentObject.transform.localPosition);
                
        TextContentTransform.anchoredPosition = 
            new Vector2(
                TextContentTransform.anchoredPosition.x - ((TextCanvas.pixelRect.width + MoveOffset) * 2.0f),
                TextContentTransform.anchoredPosition.y
            );

        // Debug.Log("Position after teleport: " + TextContentObject.transform.localPosition);
        
        yield return StartCoroutine(MoveText(Direction.Right, 0.5f, TextCanvas.pixelRect.width + MoveOffset));
        
        ScrollbarObject.gameObject.SetActive(true);

        Debug.Log("Move in done");
        
        yield break;
    }

    
    /* ---------- Event Methods ---------- */
    
    public void OnAcknowledgeButtonPressed() {

        notifyState = NotifyState.Off;
    }

    public void OnBackButtonPressed() {
        
        
    }

    public void OnForwardButtonPressed() {
        
        
    }
    
    private void OnMessageReceived(object sender, EventArgs args) {

        BoardMessageChannel.MessageSentArgs messageArgs =
            args as BoardMessageChannel.MessageSentArgs;

        if (messageArgs == null) { return; }
        
        AddMessage(messageArgs.Message);
    }

    private void OnKeycodeDiscovered(object keycode) {
        
        AddMessage(
            new Message(
                MessageKeycodeDiscoveredFistPart + (keycode as Keycode).CodeString +
                MessageKeycodeDiscoveredSecondPart
            )
        );
    }


    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}