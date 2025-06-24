using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;
using CustomEvent;
using CustomEvent.Modules;
using Microsoft.Xna.Framework;

namespace ExampleEvent
{
    public partial class TheRumbling : IInvasion
    {
        public string EventName => "The Rumbling";
        public string[] Alias => new string[] { "rumbling", "aot" };
        //To you, 2000 years from now...
        public int EventID => 2000;
        public KeyValuePair<LocalizedText, Color> WestIncomingMsg
            => new KeyValuePair<LocalizedText, Color>(new LocalizedText("The Titan has already reached the western sea!", "Rumbling.West"), new Color(255, 85, 0));
        public KeyValuePair<LocalizedText, Color> EastIncomingMsg
            => new KeyValuePair<LocalizedText, Color>(new LocalizedText("The Titan has already reached the eastern sea!", "Rumbling.East"), new Color(255, 85, 0));
        public KeyValuePair<LocalizedText, Color> ArrivedMsg
            => new KeyValuePair<LocalizedText, Color>(new LocalizedText("The Titan has reached the base!", "Rumbling.Center"), new Color(183, 55, 27));
        public KeyValuePair<LocalizedText, Color> DefeatedMsg
            => new KeyValuePair<LocalizedText, Color>(new LocalizedText("The Rumbling has been stopped!", "Rumbling.Defeated"), new Color(135, 138, 143));
        public Dictionary<int, int> EnemyPool => new Dictionary<int, int>
        {
            { NPCID.DD2OgreT3, 3 }
        };

        /// <summary>
        /// Called each time a new day started (at 4:30 am)
        /// Gets all custom events to check its event condition.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.Invasion.CheckDaytimeEventRequirementsEventArgs"/>.</param>
        internal void OnCheckEventRequirements(object? obj, ExHooks.Events.Invasion.CheckDaytimeEventRequirementsEventArgs args)
        {
            if (Main.hardMode)
            {
                Main.StartInvasion(2000);
                args.Handled = true;
            }
        }


        /// <summary>
        /// Called each time an event is about to be happened.
        /// Configures custom event properties
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.Invasion.EventConfigurationEventArgs"/>.</param>
        internal void OnEventConfiguration(object? obj, ExHooks.Events.Invasion.EventConfigurationEventArgs args)
        {
            int num = 0;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && Main.player[i].statLifeMax >= 400)
                {
                    num++;
                }
            }
            if (num > 0)
            {
                Core.eventType = args.EventID;
                InvasionCore.eventSize = 10 + 5 * TShock.Config.Settings.InvasionMultiplier * num;
                InvasionCore.eventWarn = 0;
            }
            InvasionCore.eventX = Main.rand.Next(2) == 0 ? 0.0 : (double)Main.maxTilesX;
            args.Handled = true;
        }

        /// <summary>
        /// Called each time a new event npc is about to be spawned.
        /// Gets on-going custom event to spawn its npc.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="args">Data for the <see cref=ExHooks.Events.Invasion.SpawnEventNPCEventArgs"/>.</param>
        internal void OnSpawnEventNPC(object? obj, ExHooks.Events.Invasion.SpawnEventNPCEventArgs args)
        {
            if (Core.eventType == 2000 && InvasionCore.eventSize > 0 && args.Player!.active)
            {
                if (NPC.CountNPCS(NPCID.DD2OgreT3) < 7)
                {
                    NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), args.TileX * 16 + 8, args.TileY * 16, NPCID.DD2OgreT3);
                    args.Handled = true;
                }
            }
        }

        public void OnEventCompletion() { }
    }
}
