using System.Data;
using System.Reflection;
using CustomEvent.Modules;
using CustomEvent.Patches;
using ExHooks;
using NuGet.Packaging;
using Terraria;
using TerrariaApi.Server;

namespace CustomEvent
{
    [ApiVersion(2, 1)]
    public class Core : TerrariaPlugin
    {
        public override string Author => "sors89";
        public override string Description => "Creates your own event in Terraria with TShock!";
        public override string Name => "CustomEvent";
        public override Version Version => new Version(1, 0, 2);

        /// <summary>
        /// The type of the on-going custom event. 
        /// A positive value means an invasion is in progress.
        /// A negative value means a multiwave invasion is in progress.
        /// </summary>
        public static int eventType;

        /// <summary>
        /// The list of custom invasion names that will be available in TShock's /worldevent command.
        /// </summary>
        internal static readonly List<string> _validCustomInvasions = new List<string>();

        /// <summary>
        /// The list of custom event instances that follows this plugin's standard.
        /// </summary>
        internal static HashSet<ICEvent> eventInstances = new HashSet<ICEvent>();

        /// <summary>
        /// The collection of disposable hooks used to patch several TShock functions for compatibility with this plugin.
        /// </summary>
        internal static HookHelper.DisposableHookCollection disposableHooks = new HookHelper.DisposableHookCollection();

        public Core(Main game) : base(game) => Order = 137;
        
        public override void Initialize()
        {
            RegisterCustomEvents();

            IL.Terraria.GameContent.Events.LanternNight.LanternsCanStart += Patch.ILHook_LanternsCanStart;
            IL.Terraria.GameContent.Events.MysticLogFairiesEvent.TrySpawningFairies += Patch.ILHook_TrySpawningFairies;
            IL.Terraria.IO.WorldFile.LoadWorld_Version2 += Patch.ILHook_LoadWorld_Version2;
            IL.Terraria.IO.WorldFile.SaveWorld_Version2 += Patch.ILHook_SaveWorld_Version2;
            IL.Terraria.Main.StartInvasion += Patch.ILHook_StartInvasion;
            IL.Terraria.MessageBuffer.GetData += Patch.ILHook_GetData;
            IL.Terraria.NPC.AnyDanger += Patch.ILHook_AnyDanger;
            IL.Terraria.NPC.BusyWithAnyInvasionOfSorts += Patch.ILHook_BusyWithAnyInvasionOfSorts;
            IL.Terraria.NPC.Collision_DecideFallThroughPlatforms += Patch.ILHook_Collision_DecideFallThroughPlatforms;
            IL.Terraria.NPC.SpawnNPC += Patch.ILHook_SpawnNPC;
            IL.Terraria.WorldGen.clearWorld += Patch.ILHook_clearWorld;
            IL.Terraria.WorldGen.SpawnTravelNPC += Patch.ILHook_SpawnTravelNPC;
            IL.Terraria.WorldGen.UpdateWorld += Patch.ILHook_UpdateWorld;

            ExHooks.Events.Invasion.EventPreparation += Handler.MyHook_OnEventPreparation;
            ExHooks.Events.Invasion.EventConfiguration += Handler.MyHook_OnEventConfiguration;
            ExHooks.Events.Invasion.EventFlagCheck += Handler.MyHook_OnEventFlagCheck;
            ExHooks.Events.Invasion.ModdedEventAnnouncement += Handler.MyHook_OnModdedEventAnnouncement;
            ExHooks.Events.Invasion.PostEventNpcKilled += Handler.MyHook_OnPostEventNPCKilled;
            ExHooks.Events.Invasion.PostEventUpdate += Handler.MyHook_OnPostEventUpdate;

            disposableHooks.ILHook(typeof(TShockAPI.Commands), "Invade", Patch.ILHook_Invade);
            disposableHooks.ILHook(typeof(TShockAPI.Commands), "ManageWorldEvent", Patch.ILHook_ManageWorldEvent);
            disposableHooks.ILHook<TShockAPI.TShock>("NpcHooks_OnStrikeNpc", Patch.ILHook_NpcHooks_OnStrikeNpc);
            disposableHooks.MMHook(typeof(TShockAPI.Commands), "Invade", Patch.MMHook_Invade);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                eventInstances.Clear();
                _validCustomInvasions.Clear();

                IL.Terraria.GameContent.Events.LanternNight.LanternsCanStart -= Patch.ILHook_LanternsCanStart;
                IL.Terraria.GameContent.Events.MysticLogFairiesEvent.TrySpawningFairies -= Patch.ILHook_TrySpawningFairies;
                IL.Terraria.IO.WorldFile.LoadWorld_Version2 -= Patch.ILHook_LoadWorld_Version2;
                IL.Terraria.IO.WorldFile.SaveWorld_Version2 -= Patch.ILHook_SaveWorld_Version2;
                IL.Terraria.Main.StartInvasion -= Patch.ILHook_StartInvasion;
                IL.Terraria.MessageBuffer.GetData -= Patch.ILHook_GetData;
                IL.Terraria.NPC.AnyDanger -= Patch.ILHook_AnyDanger;
                IL.Terraria.NPC.BusyWithAnyInvasionOfSorts -= Patch.ILHook_BusyWithAnyInvasionOfSorts;
                IL.Terraria.NPC.Collision_DecideFallThroughPlatforms -= Patch.ILHook_Collision_DecideFallThroughPlatforms;
                IL.Terraria.NPC.SpawnNPC -= Patch.ILHook_SpawnNPC;
                IL.Terraria.WorldGen.clearWorld -= Patch.ILHook_clearWorld;
                IL.Terraria.WorldGen.SpawnTravelNPC -= Patch.ILHook_SpawnTravelNPC;
                IL.Terraria.WorldGen.UpdateWorld -= Patch.ILHook_UpdateWorld;

                ExHooks.Events.Invasion.EventPreparation -= Handler.MyHook_OnEventPreparation;
                ExHooks.Events.Invasion.EventConfiguration -= Handler.MyHook_OnEventConfiguration;
                ExHooks.Events.Invasion.EventFlagCheck -= Handler.MyHook_OnEventFlagCheck;
                ExHooks.Events.Invasion.ModdedEventAnnouncement -= Handler.MyHook_OnModdedEventAnnouncement;
                ExHooks.Events.Invasion.PostEventNpcKilled -= Handler.MyHook_OnPostEventNPCKilled;
                ExHooks.Events.Invasion.PostEventUpdate -= Handler.MyHook_OnPostEventUpdate;

                disposableHooks.Dispose();
            }
            base.Dispose(disposing);
        }

        public static void RegisterCustomEvents()
        {
            //Thank you SGKoishi
            var loadedAssemblies = ((Dictionary<string, Assembly>)typeof(ServerApi)
                        .GetField("loadedAssemblies", BindingFlags.NonPublic | BindingFlags.Static)!
                        .GetValue(null)!)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)!;

            foreach (var assembly in loadedAssemblies.Values)
            {
                var cevents = assembly.GetTypes()
                        .Where(x => typeof(ICEvent).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                        .Select(x => (ICEvent)Activator.CreateInstance(x)!)
                        .ToHashSet();

                if (cevents.Count > 0)
                {
                    eventInstances.AddRange(cevents);
                }
            }

            TShockAPI.TShock.Log.ConsoleInfo($"[CustomEvent] --> Loaded {eventInstances.Count} custom event(s).");

            var aliases = eventInstances.Select(ev => ev.Alias.First().ToLowerInvariant());
            _validCustomInvasions.AddRange(aliases);
        }
    }
}
