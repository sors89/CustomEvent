using Terraria;
using Terraria.ID;
using TShockAPI;

namespace CustomEvent.ExampleEvent
{
    public class TheRumbling : ICEvent
    {
        public string Name => "The Rumbling";
        //To you, 2000 years from now...
        public int EventID => 2000;
        public List<int> EnemyPool => new List<int>() { NPCID.DD2OgreT3 };

        public void ConfigureEvent(int eventId, ref bool handled)
        {
            if (eventId != EventID)
            {
                handled = true;
                return;
            }

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
                Core.eventSize = 10 + 5 * num;
            }
            Core.eventX = (double)(Main.spawnTileX - 1);
            handled = false;
        }

        public void CheckRequirementsForDaytimeEvent(ref bool handled)
        {
            if (Main.hardMode)
            {
                Main.StartInvasion(EventID);
                TSPlayer.All.SendMessage($"{Name} has started... Could this mark the end of humanity?", 255, 85, 0);
                handled = true;
            }
            else
            {
                handled = false;
            }
        }

        public void CheckRequirementsForNighttimeEvent(ref bool handled) { }

        public void SpawnEventNPC(Player player, int tileX, int tileY, ref bool handled)
        {
            if (NPC.CountNPCS(NPCID.DD2OgreT3) < Core.eventSize)
            {
                NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), tileX * 16 + 8, tileY * 16, NPCID.DD2OgreT3);
            }
            handled = true;
        }
    }
}
