using System;
using UnityEngine;


namespace VirtualSelf.GameSystems {

    
/// <summary>
/// TODO: Fill out this class description: MessageChannel
/// </summary>
[CreateAssetMenu(
    fileName = nameof(BoardMessageChannel),
    menuName = ("InformationBoard/" + nameof(BoardMessageChannel))
)]
public sealed class BoardMessageChannel : ScriptableObject {

    /* ---------- Variables & Properties ---------- */

    
    
    
    /* ---------- Events & Delegates ---------- */

    public event EventHandler MessageSent;



    /* ---------- Methods ---------- */

    public void SendMessage(Message message) {
        
        MessageSent?.Invoke(this, new MessageSentArgs(message));
    }


    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */

    public class MessageSentArgs : EventArgs {

        public Message Message { get; }

        public MessageSentArgs(Message message) { Message = message; }
    }




}

}