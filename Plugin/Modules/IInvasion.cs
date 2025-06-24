using Terraria.Localization;
using Microsoft.Xna.Framework;

namespace CustomEvent.Modules
{
    /// <summary>
    /// An interface for defining custom invasion.
    /// </summary>
    public interface IInvasion : ICEvent
    {
        /// <summary>
        /// The message that will be announced to all players as the event approaches from the west.<br/>
        /// TKey: The annoucement message.<br/>
        /// TValue: The color of the message.
        /// </summary>
        KeyValuePair<LocalizedText, Color> WestIncomingMsg { get; }

        /// <summary>
        /// The message that will be announced to all players as the event approaches from the east.<br/>
        /// TKey: The annoucement message.<br/>
        /// TValue: The color of the message.
        /// </summary>
        KeyValuePair<LocalizedText, Color> EastIncomingMsg { get; }

        /// <summary>
        /// The message that will be announced to all players when the event arrives.<br/>
        /// TKey: The annoucement message.<br/>
        /// TValue: The color of the message.
        /// </summary>
        KeyValuePair<LocalizedText, Color> ArrivedMsg { get; }

        /// <summary>
        /// The message that will be announced to all players when the event has ended.<br/>
        /// TKey: The annoucement message.<br/>
        /// TValue: The color of the message.
        /// </summary>
        KeyValuePair<LocalizedText, Color> DefeatedMsg { get; }

        /// <summary>
        /// A method that is triggered each time a custom event has been defeated.
        /// </summary>
        void OnEventCompletion();
    }
}
