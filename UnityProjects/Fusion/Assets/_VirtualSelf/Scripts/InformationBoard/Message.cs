using System;


namespace VirtualSelf.GameSystems {

    
/// <summary>
/// This class models a simple "Message", to be displayed on the "Information Board" within each
/// room of the game.<br/>
/// Each message contains the text to be displayed on the board, and some additional control
/// parameters for how the board should react upon receiving that message.<br/>
/// <br/>
/// For more details about how the board handles the messages, see the
/// <see cref="InformationBoardController"/> class.<br/>
/// For how to send <see cref="Message"/> instances to the board, see the
/// <see cref="BoardMessageChannel"/> class.
/// </summary>
[Serializable]
public sealed class Message {

    /* ---------- Variables & Properties ---------- */

    /// <summary>
    /// The text of this message. This is displayed on the board. Simple "rich text" syntax can be
    /// used here (see: https://docs.unity3d.com/Manual/StyledText.html).
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// The additional control options for the board for this message.
    /// </summary>
    public BoardControlOptions ControlOptions { get; }


    /* ---------- Constructors ---------- */

    public Message(string text, BoardControlOptions controlOptions) {

        Text = text;
        ControlOptions = controlOptions;
    }


    /* ---------- Inner Classes ---------- */

    /// <summary>
    /// A list of additional options to control how the board is supposed to react upon receiving
    /// this message.<br/>
    /// Instances of this class can be reused between <see cref="Message"/> instances.
    /// </summary>
    [Serializable]
    public class BoardControlOptions {

        /* ---------- Variables & Properties ---------- */

        /// <summary>
        /// Whether to notify the player when this message arrives at the board, or not. If this is
        /// <c>true</c>, the board will go into "notification mode" (see the
        /// <see cref="InformationBoardController"/> class for details) when the message arrives.
        /// </summary>
        public bool NotifyPlayer { get; set; } = true;

        /// <summary>
        /// Whether the board should immediately switch to this message when it arrives on it,
        /// instead of waiting for the player to (hopefully) do that eventually, or not.<br/>
        /// (For details on what that means, see the <see cref="InformationBoardController"/>
        /// class).
        /// </summary>
        public bool SwitchToMessage { get; set; } = true;
    }
}

}