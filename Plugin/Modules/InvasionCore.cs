
namespace CustomEvent.Modules
{
    public class InvasionCore
    {
        /// <summary>
        /// The total npcs of the event.
        /// Decreases each time an event npc is killed. Once the count reaches 0, the event will end.
        /// </summary>
        public static int eventSize;

        /// <summary>
        /// The total npcs of the event.
        /// Stores the total npc of the event, used to calculate the current event progress percentage.
        /// </summary>
        public static int eventSizeStart;

        /// <summary>
        /// The cooldown between each warning message as the event approaches.
        /// </summary>
        public static int eventWarn;

        /// <summary>
        /// The event's X coordinate.
        /// </summary>
        public static double eventX;
    }
}
