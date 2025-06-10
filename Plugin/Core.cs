using System.Reflection;
using Terraria;
using TerrariaApi.Server;

namespace CustomEvent
{
    [ApiVersion(2, 1)]
    public partial class Core : TerrariaPlugin
    {
        public override string Author => "sors89";
        public override string Description => "Creates your own event in Terraria with TShock!";
        public override string Name => "CustomEvent";
        public override Version Version => new Version(1, 0, 0);

        /// <summary>
        /// The type of the on-going custom event. 
        /// </summary>
        public static int eventType;

        /// <summary>
        /// The total enemies of the event.
        /// Decreases each time an event npc is killed. Once the count reaches 0, the event will end.
        /// </summary>
        public static int eventSize;

        /// <summary>
        /// The cooldown between each warning message as the event approaches.
        /// </summary>
        public static int eventWarn;

        /// <summary>
        /// The event's X coordinate.
        /// </summary>
        public static double eventX;

        /// <summary>
        /// The list of custom event's instance that follows this plugin's standard.
        /// </summary>
        internal static HashSet<ICEvent>? eventInstances;

        public Core(Main game) : base(game)
        {
            eventInstances = Assembly.GetExecutingAssembly().GetTypes()
                        .Where(x => typeof(ICEvent).IsAssignableFrom(x) && x != typeof(ICEvent))
                        .Select(x => Activator.CreateInstance(x)).Cast<ICEvent>()
                        .ToHashSet();
        }

        public override void Initialize()
        {
            ExHooks.Events.Invasion.EventPreparation += Handler.MyHook_OnEventPreparation;
            ExHooks.Events.Invasion.EventConfiguration += Handler.MyHook_OnEventConfiguration;
            ExHooks.Events.Invasion.EventFlagCheck += Handler.MyHook_OnEventFlagCheck;
            ExHooks.Events.Invasion.ModdedEventAnnouncement += Handler.MyHook_OnModdedEventAnnouncement;
            ExHooks.Events.Invasion.SpawnEventNPC += Handler.MyHook_OnSpawnEventNPC;
            ExHooks.Events.Invasion.PostEventNpcKilled += Handler.MyHook_OnPostEventNPCKilled;
            ExHooks.Events.Invasion.ModdedEventUpdate += Handler.MyHook_OnModdedEventUpdate;
            ExHooks.Events.World.CheckDaytimeEventRequirements += Handler.MyHook_OnCheckDaytimeEventRequirements;
            ExHooks.Events.World.CheckNighttimeEventRequirements += Handler.MyHook_OnCheckNighttimeEventRequirements;
        }
    }
}
