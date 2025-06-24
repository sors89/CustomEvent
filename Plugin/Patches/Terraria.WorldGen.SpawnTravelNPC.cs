using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_SpawnTravelNPC(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(i => i.MatchRet());

            //Gotta do the modern way here because the old-fashioned way is way too complicated.
            if (csr.Previous.Operand is ILLabel retargetLabel)
            {
                csr.Index++;
                //Rewrite the whole logic
                csr.RemoveRange(9);

                var parentLabel = csr.DefineLabel();
                csr.MarkLabel(retargetLabel);
                csr.Emit(OpCodes.Ldsfld, CachedFields.invasionDelay);
                csr.Emit(OpCodes.Brtrue_S, parentLabel);

                var vchildLabel = csr.DefineLabel();
                csr.Emit(OpCodes.Ldsfld, CachedFields.invasionType);
                csr.Emit(OpCodes.Ldc_I4_0);
                csr.Emit(OpCodes.Ble_S, vchildLabel);
                csr.Emit(OpCodes.Ldsfld, CachedFields.invasionSize);
                csr.Emit(OpCodes.Ldc_I4_0);
                csr.Emit(OpCodes.Ble_S, vchildLabel);
                csr.Emit(OpCodes.Ret);
                csr.MarkLabel(vchildLabel);

                var mchildLabel = csr.DefineLabel();
                csr.Emit(OpCodes.Ldsfld, CachedFields.eventType);
                csr.Emit(OpCodes.Ldc_I4_0);
                csr.Emit(OpCodes.Ble_S, mchildLabel);
                csr.Emit(OpCodes.Ldsfld, CachedFields.eventSize);
                csr.Emit(OpCodes.Ldc_I4_0);
                csr.Emit(OpCodes.Ble_S, mchildLabel);
                csr.Emit(OpCodes.Ret);
                csr.MarkLabel(mchildLabel);

                csr.MarkLabel(parentLabel);
            }
        }
    }
}
