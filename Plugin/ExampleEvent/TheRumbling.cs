using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;
using Microsoft.Xna.Framework;

namespace CustomEvent.ExampleEvent
{
    public class TheRumbling : IDaytimeEvent
    {
        public string Name => "The Rumbling";
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

        public void ConfigureEvent(int eventId)
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
                Core.eventType = eventId;
                Core.eventSize = 10 + (5 * TShock.Config.Settings.InvasionMultiplier * num);
                Core.eventWarn = 0;
            }
            Core.eventX = (double)Main.maxTilesX;
        }

        public bool CheckRequirementsForDaytimeEvent()
        {
            if (Main.hardMode)
            {
                Main.StartInvasion(EventID);
                return true;
            }
            return false;
        }

        public bool SpawnEventNPC(Player player, int tileX, int tileY)
        {
            if (NPC.CountNPCS(NPCID.DD2OgreT3) < 7)
            {
                NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), tileX * 16 + 8, tileY * 16, NPCID.DD2OgreT3);
                return true;
            }
            return false;
        }

        public void OnEventCompletion() { }
    }
}
