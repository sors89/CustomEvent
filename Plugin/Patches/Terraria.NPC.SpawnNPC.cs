using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_SpawnNPC(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(MoveType.After, i => i.MatchLdsfld(CachedFields.invasionType),
                                         i => i.MatchLdcI4(0),
                                         i => i.OpCode == OpCodes.Bgt_S);

            var targetBranch = csr.Previous.Operand;
            csr.Emit(OpCodes.Ldsfld, CachedFields.eventType);
            csr.Emit(OpCodes.Ldc_I4_0);
            csr.Emit(OpCodes.Bne_Un, targetBranch);
        }
    }
}
