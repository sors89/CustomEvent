using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_LoadWorld_Version2(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            while (csr.TryGotoNext(i => i.MatchRet())) ;

            csr.Emit(OpCodes.Nop);
            csr.Emit(OpCodes.Ldarg_0);
            csr.EmitDelegate<Func<BinaryReader, int>>(reader =>
            {
                if (reader.BaseStream.Length >= reader.BaseStream.Position + 20)
                {
                    Core.eventSize = reader.ReadInt32();
                    Core.eventType = reader.ReadInt32();
                    Core.eventX = reader.ReadDouble();
                    Core.eventSizeStart = reader.ReadInt32();
                }
                return 0;
            });
            csr.Emit(OpCodes.Ret);
            csr.Emit(OpCodes.Ldc_I4_6);

            var nopIns = csr.Body.Instructions.Reverse().Where(x => x.MatchNop()).First();
            nopIns.OpCode = OpCodes.Brtrue_S;
            nopIns.Operand = csr.Previous;
        }
    }
}
