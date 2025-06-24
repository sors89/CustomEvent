using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_AnyDanger(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(MoveType.After, i => i.MatchLdsfld(CachedFields.invasionType),
                                         i => i.MatchLdcI4(0),
                                         i => i.OpCode == OpCodes.Ble_S);

            var backtrack = csr.Body.Instructions[csr.Index - 1];
            var targetLabel = csr.Previous.Operand;
            csr.Emit(OpCodes.Ldsfld, CachedFields.eventType);
            csr.Emit(OpCodes.Ldc_I4_0);
            csr.Emit(OpCodes.Ble_S, targetLabel);
            csr.Emit(OpCodes.Ldc_I4_1);
            csr.Emit(OpCodes.Stloc_0);

            csr.Goto(backtrack);
            csr.Next.Operand = csr.Next.Next;
        }
    }
}
