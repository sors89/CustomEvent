using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_TrySpawningFairies(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(MoveType.After, i => i.OpCode == OpCodes.Ble_S);

            if (csr.Previous.Operand is ILLabel needToBeRetargetedLabel)
            {
                csr.Previous.OpCode = OpCodes.Bgt_S;
                var targetLabel = csr.Previous.Operand;
                csr.Emit(OpCodes.Ldsfld, CachedFields.eventType);
                csr.Emit(OpCodes.Ldc_I4_0);
                csr.Emit(OpCodes.Ble_S, targetLabel);
                csr.MarkLabel(needToBeRetargetedLabel);
            }
        }
    }
}
