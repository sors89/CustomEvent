using Terraria;

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
        /// The id of the event.
        /// Avoid assigning the same ID as the vanilla invasion type or any other custom event.
        /// </summary>
        int EventID { get; }

        /// <summary>
        /// The list of enemies that will be spawned during the event.
        /// </summary>
        List<int> EnemyPool { get; }

        /// <summary>
        /// A method that is triggered each time an event is about to be happened.
        /// Used to configure custom event properties such as EventSize, EventX,...
        /// </summary>
        void ConfigureEvent(int eventId, ref bool handled);

        /// <summary>
        /// A method that is triggered each time a new day started (at 4:30 am).
        /// Used to check if the custom event's conditions are met.
        /// </summary>
        void CheckRequirementsForDaytimeEvent(ref bool handled);

        /// <summary>
        /// A method that is triggered each time a new night started (at 7:30 pm).
        /// Used to check if the custom event's conditions are met.
        /// </summary>
        void CheckRequirementsForNighttimeEvent(ref bool handled);

        /// <summary>
        /// A method that is triggered each time an npc is about to spawn.
        /// Used to replace placeholder npc with the event's designed npc.
        /// </summary>
        void SpawnEventNPC(Player player, int tileX, int tileY, ref bool handled);
    }
}
