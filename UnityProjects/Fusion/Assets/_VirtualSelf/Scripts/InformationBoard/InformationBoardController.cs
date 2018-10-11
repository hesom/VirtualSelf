using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Interaction;
using UnityEngine;
using VirtualSelf.Utility;


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

    public enum PanelMoveDirection {

        Left,
        Right
    }
    
    
    /* ---------- Variables & Properties ---------- */
    
    [TextArea(3,10)]
    public string KeycodeDiscoveredMessageFirstPart =
        "You have discovered a new keycode! The keycode is <b>";
    
    [TextArea(3,10)]
    public string KeycodeDiscoveredMessageSecondPart =
        "</b>.\n\nRemember that you can also check all the keycodes you have discovered, and " + 
        "their associated rooms, in the <b>Hub Room</b> or on your <b>Keycodes List</b> screen.";
       
    public LampAnimator LeftLight;
    public LampAnimator RightLight;

    public Canvas TextCanvas;

    public PanelSettings PanelPrefab;

    public ScrollRectTouchscreenControls TouchscreenControls;

    public InteractionButton ButtonPrevious;
    public InteractionButton ButtonAcknowledge;
    public InteractionButton ButtonNext;

    public BoardMessageChannel MessageChannel;

    public KeycodesList KeycodesList;

    // TODO: Make private after testing
    public NotifyState notifyState = NotifyState.Off;
    
    public float PanelsPadding = 30.0f;

    public float MessageSwitchTime = 1.0f;

    public float SwitchDelayTime = 0.5f;

    public Room CurrentRoom;
    
    private readonly List<MessagePanel> messagePanels = new List<MessagePanel>();

    private readonly List<Message> waitingMessages = new List<Message>();

    private int currentPanelIndex;

    private float singlePanelWidth;

    private float singlePanelDistance;

    private bool isCurrentlySwitching;

    private Material materialButtonPrevious;
    private Material materialButtonAcknowledge;
    private Material materialButtonNext;
    

    /* ---------- Methods ---------- */

    private void Start() {

        materialButtonPrevious = 
            new Material(ButtonPrevious.gameObject.GetComponent<Renderer>().material);
        materialButtonAcknowledge = 
            new Material(ButtonAcknowledge.gameObject.GetComponent<Renderer>().material);
        materialButtonNext = 
            new Material(ButtonNext.gameObject.GetComponent<Renderer>().material);

        ButtonPrevious.GetComponent<Renderer>().material = materialButtonPrevious;
        ButtonAcknowledge.GetComponent<Renderer>().material = materialButtonAcknowledge;
        ButtonNext.GetComponent<Renderer>().material = materialButtonNext;
        
        MaterialUtils.SetEmissionState(materialButtonPrevious, true);
        MaterialUtils.SetEmissionState(materialButtonAcknowledge, true);
        MaterialUtils.SetEmissionState(materialButtonNext, true);
        MaterialUtils.SetEmissionColorToRegularColor(materialButtonPrevious);
        MaterialUtils.SetEmissionColorToRegularColor(materialButtonAcknowledge);
        MaterialUtils.SetEmissionColorToRegularColor(materialButtonNext);
        MaterialUtils.SetEmissionValue(materialButtonPrevious, 0.0f);
        MaterialUtils.SetEmissionValue(materialButtonAcknowledge, 0.0f);
        MaterialUtils.SetEmissionValue(materialButtonNext, 0.0f);
        
        MessageChannel.MessageSent += OnMessageReceived;
        KeycodesList.OnKeycodeStateChanged.AddListener(OnKeycodeDiscovered);

//        Debug.Log("PanelPrefab \"ScrollRect.content.width\" is " + singlePanelWidth.ToString("N2"));
//        Debug.Log("Canvas \"pixelRect.width\" is " + TextCanvas.pixelRect.width.ToString("N2"));
//        Debug.Log("Canvas \"RectTransform.rect.width.width\" is " +
//                  TextCanvas.gameObject.GetComponent<RectTransform>().rect.width.ToString("N2"));
        
        singlePanelWidth = TextCanvas.gameObject.GetComponent<RectTransform>().rect.width;
        singlePanelDistance = (singlePanelWidth + PanelsPadding);

        if (CurrentRoom != null) {
            
            AddMessage(
                new Message(
                    ($"<b>{CurrentRoom.RoomName}</b>\n\n{CurrentRoom.Description}"),
                    new Message.BoardControlOptions { NotifyPlayer = false, SwitchToMessage = true } 
                )
            );
        }
        else {
            
            Debug.LogWarning(
                "This information board has no \"Room\" asset attached to it. No initial message " +
                "will be created for the current room."
            );
        }
    }

    private void Update() {

        if (notifyState == NotifyState.Off) {
            
            MaterialUtils.SetEmissionValue(materialButtonAcknowledge, 0.0f);
            // TODO: Lamps
        }
        else if (notifyState == NotifyState.Notifying) {
            
            MaterialUtils.SetEmissionValue(materialButtonAcknowledge, 0.3f);
        }

//        if (Input.GetKeyDown(KeyCode.M)) {
//
//            MaterialUtils.SetEmissionState(materialButtonAcknowledge, true);
//            MaterialUtils.SetEmissionValue(materialButtonAcknowledge, 0.4f);
//        }
//        if (Input.GetKeyDown(KeyCode.N)) {
//            
//            AddMessage(
//                new Message(
//                    "Message number " + $"{messagePanels.Count:D2}",
//                    new Message.BoardControlOptions {
//                        NotifyPlayer = false, SwitchToMessage = true
//                    }
//                )
//            );
//        }
//        if (Input.GetKeyDown(KeyCode.G)) {
//
//            SwitchToPreviousMessage();
//        }
//        if (Input.GetKeyDown(KeyCode.H)) {
//
//            SwitchToNextMessage();
//        }

        if ((waitingMessages.Count > 0) && (isCurrentlySwitching == false)) {

            Message newMessage = waitingMessages[0];
            waitingMessages.RemoveAt(0);
            
            AddMessage(newMessage);
        }
    }

    /// <summary>
    /// Adds the new given message to the board. TODO: More
    /// </summary>
    /// <remarks>
    /// Notice that this class makes no attempt to check for duplicate messages. If this message has
    /// already been added to the board before, there will now be two separate instances of it
    /// within it (in different positions).
    /// </remarks>
    /// <param name="message">The message to add to the board.</param>
    private void AddMessage(Message message) {

        /* If we are currently switching, we need to store the message and try again after that. */
        
        if (isCurrentlySwitching == true) {
            
            Debug.LogWarning(
                "The Information Board is currently already in the process of switching the " +
                "shown message. A new message can not be added to it right now. The message will " +
                "be added once the switching is done."
            );
            
            waitingMessages.Add(message);
            return;
        }
        
        PanelSettings newSettings = Instantiate(PanelPrefab, TextCanvas.transform);
        
        newSettings.TextContentTransform.anchoredPosition = 
            new Vector2(CalculateNewPanelDistance(), 0.0f);
        
        newSettings.TextObject.text = message.Text;

        newSettings.gameObject.name = 
            ("Message Panel " + $"{messagePanels.Count:D2}" + " (Instance)");
        
        newSettings.gameObject.SetActive(true);
        
        messagePanels.Add(new MessagePanel(message, newSettings));
        
        /* Activate the message board lamps if the message wants us to. */

        if (message.ControlOptions.NotifyPlayer) { notifyState = NotifyState.Notifying; }

        /* Switch to the newly added message if the message wants us to. */
        
        if (message.ControlOptions.SwitchToMessage) {

            /* This is simply the last element in our panels list, as we only ever append panels
             * at the end in this class. */
            
            SwitchToMessage(messagePanels.Count - 1);
        }
    }

    private void SwitchToNextMessage() {

        if (isCurrentlySwitching) { return; }

        if (currentPanelIndex >= (messagePanels.Count - 1)) { return; }

        SwitchToMessage(currentPanelIndex + 1);
    }

    private void SwitchToPreviousMessage() {
        
        if (isCurrentlySwitching) { return; }

        if (currentPanelIndex == 0) { return; }       
        
        SwitchToMessage(currentPanelIndex - 1);
    }

    private void SwitchToMessage(int newMessageIndex) {
        
        if (newMessageIndex >= messagePanels.Count) {
            
            Debug.LogWarning(
                "There is no message stored in the Information Board with an index of " +
                newMessageIndex + ". The highest index is " + messagePanels.Count + ". Could not " +
                "switch the message."
            );
            return;
        }
        if (isCurrentlySwitching) {

            Debug.LogWarning(
                "The Information Board is currently already switching the shown message. Cannot " +
                "switch again until this is finished."
            );
            return;
        }

        StartCoroutine(SwitchToMessageRoutine(newMessageIndex));
    }
    
    private IEnumerator SwitchToMessageRoutine(int newMessageIndex) {

        /* We are now starting with switching panels. */
        
        isCurrentlySwitching = true;
        
        /* We need to deactivate the touchscreen controls here, since we don't want to be able to
         * use the touchscreen while the contents are changing.
         * This is also important because we will need to switch the ("ScrollRect") reference the
         * touchscreen controls are using once we are done switching panels. */

        TouchscreenControls.IsActivated = false;
        
        /* Before we actually switch, we want to disable the scrolling functionality and especially
         * hide the scrollbar of the currently shown panel, until we are done with switching. */
        
        int oldMessageIndex = currentPanelIndex;
        currentPanelIndex = newMessageIndex;

        MessagePanel oldPanel = messagePanels[oldMessageIndex];
        
        oldPanel.PanelSettings.ScrollRectScrollbar.gameObject.SetActive(false);

        oldPanel.PanelSettings.TextScrollRect.verticalScrollbar = null;
        oldPanel.PanelSettings.TextScrollRect.vertical = false;

        /* Switch all panels, until we arrive at the one given by the new index. */
        
        PanelMoveDirection switchDirection =
            (oldMessageIndex < currentPanelIndex) ? 
                PanelMoveDirection.Left : 
                PanelMoveDirection.Right;

        int switchCount = Math.Abs(oldMessageIndex - currentPanelIndex);

        for (int i = 0; i < switchCount; i++) {

            MoveAllPanels(switchDirection);

            if (i != (switchCount - 1)) {
                yield return (new WaitForSeconds(MessageSwitchTime + SwitchDelayTime));
            }
            else {
                yield return (new WaitForSeconds(MessageSwitchTime));               
            }
        }
        
        /* Now that we are done switching, we can make the new current Message Panel its
         * scrollbar. */
        
        MessagePanel newPanel = messagePanels[currentPanelIndex];

        newPanel.PanelSettings.ScrollRectScrollbar.gameObject.SetActive(true);
        
        newPanel.PanelSettings.TextScrollRect.verticalScrollbar =
            newPanel.PanelSettings.ScrollRectScrollbar;
        newPanel.PanelSettings.TextScrollRect.vertical = true;

        /* Now, as a final action, give the "TouchscreenControls" a new reference to the
         * "ScrollRect" of the new active panel. Then, activate them again. */
        
        TouchscreenControls.ScrollRect = newPanel.PanelSettings.TextScrollRect;

        TouchscreenControls.IsActivated = true;

        /* We are now done with switching. */
        
        isCurrentlySwitching = false;
    }

    /// <summary>
    /// Moves all <see cref="MessagePanel"/>s in <see cref="messagePanels"/> one "step" (meaning,
    /// the distance of <see cref="singlePanelDistance"/> into the given direction.
    /// </summary>
    /// <remarks>
    /// All panels will be moved at the same time, not one after another.<br/>
    /// This starts multiple coroutines, which will (together) take as long as
    /// <see cref="MessageSwitchTime"/> to complete the task. The method does not wait for this.
    /// </remarks>
    /// <param name="direction">The direction to move all the panels into.</param>
    private void MoveAllPanels(PanelMoveDirection direction) {

        foreach (MessagePanel panel in messagePanels) {

            MovePanel(panel, direction);
        }
    }

    /// <summary>
    /// Moves a single <see cref="MessagePanel"/> one "step" (meaning, the distance of
    /// <see cref="singlePanelDistance"/>) into the given direction.
    /// </summary>
    /// <remarks>
    /// This starts a coroutine, which will take as long as <see cref="MessageSwitchTime"/> to
    /// complete the task. The method does not wait for this.
    /// </remarks>
    /// <param name="panel">The message panel to move.</param>
    /// <param name="direction">The direction to move <paramref name="panel"/> into.</param>
    private void MovePanel(MessagePanel panel, PanelMoveDirection direction) {

        Vector2 sourcePos = panel.PanelSettings.TextContentTransform.anchoredPosition;
        Vector2 destPos;

        if (direction == PanelMoveDirection.Left) {
            
            destPos = new Vector2(sourcePos.x - singlePanelDistance, sourcePos.y);
        }
        else if (direction == PanelMoveDirection.Right) {
            
            destPos = new Vector2(sourcePos.x + singlePanelDistance, sourcePos.y);           
        }
        else {
            
            throw new NotImplementedException();
        }

        StartCoroutine(
            AnimationUtils.MoveUiElementFromToInTime(
                panel.PanelSettings.TextContentTransform,
                sourcePos, destPos, MessageSwitchTime
            )
        );
    }
    
    /// <summary>
    /// Calculates the distance between the (origin of the) message board screen, and a newly added
    /// panel GameObject (from the "MessagePanel" prefab). This will give the correct distance no
    /// matter how many panels are in between the screen and the new panel.
    /// </summary>
    /// <returns>The distance between the message board screen and a newly added panel.</returns>
    private float CalculateNewPanelDistance() {
        
        /* This is one less in total because the new panel has already been added at this point, and
         * we don't want to count it here of course. */
        int totalCount = (messagePanels.Count);

        int steps = (totalCount - currentPanelIndex);

        return (steps * singlePanelDistance);
    }

    
    /* ---------- Event Methods ---------- */
    
    public void OnAcknowledgeButtonPressed() {

        notifyState = NotifyState.Off;
    }

    public void OnBackButtonPressed() {

        if (isCurrentlySwitching == false) {
            
            SwitchToPreviousMessage();
        }
    }

    public void OnForwardButtonPressed() {

        if (isCurrentlySwitching == false) {
            
            SwitchToNextMessage();
        }
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
                (KeycodeDiscoveredMessageFirstPart + (keycode as Keycode).CodeString +
                KeycodeDiscoveredMessageSecondPart),
                new Message.BoardControlOptions()
            )
        );
    }


    /* ---------- Inner Classes ---------- */

    /// <summary>
    /// This class models a single "message panel", one "page" within the "memory" of the board,
    /// holding a single (complete) <see cref="Message"/>.<br/>
    /// The board stores instances of this class, and switches between them at runtime. For each
    /// added message, one instance of this class is created and stored within the board.
    /// </summary>
    class MessagePanel {

        public Message Message { get; }
        public PanelSettings PanelSettings { get; }

        public MessagePanel(Message message, PanelSettings panelSettings) {

            Message = message;
            PanelSettings = panelSettings;
        }
    }
}

}