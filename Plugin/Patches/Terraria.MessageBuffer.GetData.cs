using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_GetData(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(MoveType.After, i => i.OpCode == OpCodes.Ble_S,
                                         i => i.MatchLdsfld(CachedFields.invasionType),
                                         i => i.OpCode == OpCodes.Brtrue_S);

            var targetBranch = csr.Previous.Operand;
            csr.Emit(OpCodes.Ldsfld, CachedFields.eventType);
            csr.Emit(OpCodes.Brtrue_S, targetBranch);
        }
    }
}
