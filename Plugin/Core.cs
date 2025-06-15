using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using ExHooks;
using CustomEvent.Patches;

namespace CustomEvent
{
    [ApiVersion(2, 1)]
    public class Core : TerrariaPlugin
    {
        public override string Author => "sors89";
        public override string Description => "Creates your own event in Terraria with TShock!";
        public override string Name => "CustomEvent";
        public override Version Version => new Version(1, 0, 1);

        /// <summary>
        /// The type of the on-going custom event. 
        /// </summary>
        public static int eventType;

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

        /// <summary>
        /// The list of custom event instances that follows this plugin's standard.
        /// </summary>
        internal static HashSet<ICEvent>? eventInstances;

        /// <summary>
        /// The list of custom invasion names that will be available in TShock's /worldevent command.
        /// </summary>
        internal static readonly List<string> _validCustomInvasions = new List<string>();

        /// <summary>
        /// The collection of disposable hooks used to patch several TShock functions for compatibility with this plugin.
        /// </summary>
        internal static HookHelper.DisposableHookCollection disposableHooks = new HookHelper.DisposableHookCollection();

        public Core(Main game) : base(game)
        {
            eventInstances = Assembly.GetExecutingAssembly().GetTypes()
                        .Where(x => typeof(ICEvent).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                        .Select(Activator.CreateInstance).OfType<ICEvent>()
                        .ToHashSet();

            var aliases = eventInstances.Select(ev => ev.Alias.First().ToLowerInvariant());
            _validCustomInvasions.AddRange(aliases);
        }
        
        public override void Initialize()
        {
            IL.Terraria.IO.WorldFile.LoadWorld_Version2 += Patch.ILHook_LoadWorld_Version2;
            IL.Terraria.IO.WorldFile.SaveWorld_Version2 += Patch.ILHook_SaveWorld_Version2;

            ExHooks.Events.Invasion.EventPreparation += Handler.MyHook_OnEventPreparation;
            ExHooks.Events.Invasion.EventConfiguration += Handler.MyHook_OnEventConfiguration;
            ExHooks.Events.Invasion.EventFlagCheck += Handler.MyHook_OnEventFlagCheck;
            ExHooks.Events.Invasion.ModdedEventAnnouncement += Handler.MyHook_OnModdedEventAnnouncement;
            ExHooks.Events.Invasion.SpawnEventNPC += Handler.MyHook_OnSpawnEventNPC;
            ExHooks.Events.Invasion.PostEventNpcKilled += Handler.MyHook_OnPostEventNPCKilled;
            ExHooks.Events.Invasion.PostEventUpdate += Handler.MyHook_OnPostEventUpdate;
            ExHooks.Events.World.CheckDaytimeEventRequirements += Handler.MyHook_OnCheckDaytimeEventRequirements;
            ExHooks.Events.World.CheckNighttimeEventRequirements += Handler.MyHook_OnCheckNighttimeEventRequirements;

            disposableHooks.ILHook<TShockAPI.TShock>("NpcHooks_OnStrikeNpc", Patch.ILHook_NpcHooks_OnStrikeNpc);
            disposableHooks.ILHook(typeof(TShockAPI.Commands), "ManageWorldEvent", Patch.ILHook_ManageWorldEvent);
            disposableHooks.ILHook(typeof(TShockAPI.Commands), "Invade", Patch.ILHook_Invade);
            disposableHooks.MMHook(typeof(TShockAPI.Commands), "Invade", Patch.MMHook_Invade);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IL.Terraria.IO.WorldFile.LoadWorld_Version2 -= Patch.ILHook_LoadWorld_Version2;
                IL.Terraria.IO.WorldFile.SaveWorld_Version2 -= Patch.ILHook_SaveWorld_Version2;

                ExHooks.Events.Invasion.EventPreparation -= Handler.MyHook_OnEventPreparation;
                ExHooks.Events.Invasion.EventConfiguration -= Handler.MyHook_OnEventConfiguration;
                ExHooks.Events.Invasion.EventFlagCheck -= Handler.MyHook_OnEventFlagCheck;
                ExHooks.Events.Invasion.ModdedEventAnnouncement -= Handler.MyHook_OnModdedEventAnnouncement;
                ExHooks.Events.Invasion.SpawnEventNPC -= Handler.MyHook_OnSpawnEventNPC;
                ExHooks.Events.Invasion.PostEventNpcKilled -= Handler.MyHook_OnPostEventNPCKilled;
                ExHooks.Events.Invasion.PostEventUpdate -= Handler.MyHook_OnPostEventUpdate;
                ExHooks.Events.World.CheckDaytimeEventRequirements -= Handler.MyHook_OnCheckDaytimeEventRequirements;
                ExHooks.Events.World.CheckNighttimeEventRequirements -= Handler.MyHook_OnCheckNighttimeEventRequirements;

                disposableHooks.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
