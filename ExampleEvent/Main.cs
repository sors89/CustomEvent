using Terraria;
using TerrariaApi.Server;

namespace ExampleEvent
{
    [ApiVersion(2, 1)]
    public partial class TheRumbling : TerrariaPlugin
    {
        public override string Author => "sors89";
        public override string Description => "An example custom event to demonstrate the CustomEvent plugin.";
        public override string Name => "The Rumbling";
        public override Version Version => new Version(1, 0, 0);

        public TheRumbling(Main game) : base(game) { }

        public override void Initialize()
        {
            ExHooks.Events.Invasion.CheckDaytimeEventRequirements += this.OnCheckEventRequirements;
            ExHooks.Events.Invasion.EventConfiguration += this.OnEventConfiguration;
            ExHooks.Events.Invasion.SpawnEventNPC += this.OnSpawnEventNPC;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ExHooks.Events.Invasion.CheckDaytimeEventRequirements -= this   .OnCheckEventRequirements;
                ExHooks.Events.Invasion.EventConfiguration -= this.OnEventConfiguration;
                ExHooks.Events.Invasion.SpawnEventNPC -= this.OnSpawnEventNPC;
            }
            base.Dispose(disposing);
        }
    }
}
