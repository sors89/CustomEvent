using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;

namespace CustomEvent
{
    /// <summary>
    /// An interface for defining custom event.
    /// </summary>
    public interface ICEvent
    {
        /// <summary>
        /// The name of the event.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The shorter version of the event name.<br/>
        /// These aliases can be used as an alternative way to summon custom event directly via TShock's /worldevent command.
        /// </summary>
        string[] Alias { get; }

        /// <summary>
        /// The id of the event.
        /// Avoid assigning the same ID as the vanilla invasion type or any other custom event.
        /// </summary>
        int EventID { get; }

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
        /// The collection of npc that will be spawned during the event.<br/>
        /// The dictionary's TKey represents the NPC's type.<br/>
        /// The dictionary's TValue measures how difficult the npc is.
        /// The higher the value, the more are subtracted from <see cref="Core.eventSize"/> when the npc is killed.
        /// Therefore, killing npc with higher size speeds up event completion.
        /// </summary>
        Dictionary<int, int> EnemyPool { get; }

        /// <summary>
        /// A method that is triggered each time an event is about to be happened.
        /// Used to configure custom event properties such as EventSize, EventX,...
        /// </summary>
        void ConfigureEvent(int eventId);

        /// <summary>
        /// A method that is triggered each time an npc is about to spawn.
        /// Used to replace placeholder npc with the event's designed npc.
        /// </summary>
        /// <returns>True if the event npc can be spawned. Otherwise, false.</returns>
        bool SpawnEventNPC(Player player, int tileX, int tileY);

        /// <summary>
        /// A method that is triggered each time a custom event has been defeated.
        /// Has various uses depending on what you want to do.
        /// </summary>
        void OnEventCompletion();
    }

    public interface IDaytimeEvent : ICEvent
    {
        /// <summary>
        /// A method that is triggered each time a new day started (at 4:30 am).
        /// Used to check if the custom event's conditions are met.
        /// </summary>
        /// <returns>True if the event conditions are met. Otherwise, false.</returns>
        bool CheckRequirementsForDaytimeEvent();
    }

    public interface INighttimeEvent : ICEvent
    {
        /// <summary>
        /// A method that is triggered each time a new night started (at 7:30 pm).
        /// Used to check if the custom event's conditions are met.
        /// </summary>
        /// <returns>True if the event conditions are met. Otherwise, false.</returns>
        bool CheckRequirementsForNighttimeEvent();
    }
}
