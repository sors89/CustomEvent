using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_BusyWithAnyInvasionOfSorts(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(i => i.MatchLdsfld(typeof(Terraria.GameContent.Events.DD2Event), "Ongoing"));

            var targetLabel = csr.Previous.Operand;
            csr.Emit(OpCodes.Ldsfld, CachedFields.eventType);
            csr.Emit(OpCodes.Brtrue_S, targetLabel);
        }
    }
}
