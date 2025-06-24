using MonoMod.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_clearWorld(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(MoveType.After, i => i.MatchStsfld(typeof(Terraria.Main), "invasionSizeStart"));

            csr.EmitDelegate<Action>(() =>
            {
                CustomEvent.Core.eventType = 0;
                CustomEvent.Modules.InvasionCore.eventSize = 0;
                CustomEvent.Modules.InvasionCore.eventWarn = 0;
                CustomEvent.Modules.InvasionCore.eventX = 0.0;
                CustomEvent.Modules.InvasionCore.eventSizeStart = 0;
            });
        }
    }
}
