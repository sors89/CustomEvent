using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_StartInvasion(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(MoveType.After, i => i.MatchLdsfld(CachedFields.invasionType),
                                         i => i.OpCode == OpCodes.Brtrue);

            var targetLabel = csr.Previous.Operand;
            csr.Emit(OpCodes.Ldsfld, CachedFields.eventType);
            csr.Emit(OpCodes.Brtrue, targetLabel);
        }
    }
}
