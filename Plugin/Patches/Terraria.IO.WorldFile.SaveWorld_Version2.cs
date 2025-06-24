using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal partial class Patch
    {
        internal static void ILHook_SaveWorld_Version2(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(MoveType.After, i => i.MatchPop());

            csr.Emit(OpCodes.Ldarg_0);
            csr.EmitDelegate<Func<BinaryWriter, int>>(writer =>
            {
                writer.Write(CustomEvent.Modules.InvasionCore.eventSize);
                writer.Write(CustomEvent.Core.eventType);
                writer.Write(CustomEvent.Modules.InvasionCore.eventX);
                writer.Write(CustomEvent.Modules.InvasionCore.eventSizeStart);
                return (int)writer.BaseStream.Position;
            });
            csr.Emit(OpCodes.Pop);
        }
    }
}
