
namespace CustomEvent.Modules
{
    /// <summary>
    /// An interface for defining custom event.
    /// </summary>
    public interface ICEvent
    {
        /// <summary>
        /// The name of the event.
        /// </summary>
        string EventName { get; }

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
        /// The collection of npc that will be spawned during the event.<br/>
        /// The dictionary's TKey represents the NPC's type.<br/>
        /// The dictionary's TValue measures how difficult the npc is.
        /// The higher the value, the more are subtracted from <see cref="Core.eventSize"/> when the npc is killed.
        /// Therefore, killing npc with higher size speeds up event completion.
        /// </summary>
        Dictionary<int, int> EnemyPool { get; }
    }
}
