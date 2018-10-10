using System;

namespace VirtualSelf.GameSystems {

    
/// <summary>
/// TODO: Fill out this class description: Message
/// </summary>
[Serializable]
public sealed class Message {

    /* ---------- Variables & Properties ---------- */

    public string Text { get; }
    
    public bool NotifyPlayer { get; }


    /* ---------- Constructors ---------- */

    public Message(string text, bool notifyPlayer = true) {

        Text = text;
        NotifyPlayer = notifyPlayer;
    }




    /* ---------- Methods ---------- */






    /* ---------- Overrides ---------- */






    /* ---------- Inner Classes ---------- */






}

}