using ModFramework;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_NpcHooks_OnStrikeNpc(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            while (csr.TryGotoNext(MoveType.After, i => i.OpCode == OpCodes.Brfalse_S)) ;

            //Adds logic to check whether Main.invasionType > 0 to avoid accidentally setting the vanilla invasion size to 20_000_000.
            var dumbLabel = csr.Previous.Operand;
            csr.Emit(OpCodes.Ldsfld, typeof(Terraria.Main).GetField("invasionType"));
            csr.Emit(OpCodes.Ldc_I4_0);
            csr.Emit(OpCodes.Ble_S, dumbLabel);

            while (csr.TryGotoNext(i => i.MatchNop())) ;

            var targetNopLabel = csr.Next;
            csr.Emit(OpCodes.Ldsfld, typeof(CustomEvent.Core).GetField("eventSize"));
            csr.Emit(OpCodes.Ldc_I4, 10);
            csr.Emit(OpCodes.Nop);
            csr.Emit(OpCodes.Ldc_I4, 20_000_000);
            csr.Emit(OpCodes.Stsfld, typeof(CustomEvent.Core).GetField("eventSize"));

            var wrongTargetLabels = csr.Body.Instructions
                                .Where(i => i.Operand is ILLabel ilLabel && ilLabel.Target == targetNopLabel)
                                .ToList();

            for (int i = 0; i < wrongTargetLabels.Count; i++)
            {
                wrongTargetLabels[i].Operand = csr.Previous.Previous(4);
            }

            csr.Previous.Previous(2).OpCode = OpCodes.Bge_S;
            csr.Previous.Previous(2).Operand = targetNopLabel;
        }
    }
}
