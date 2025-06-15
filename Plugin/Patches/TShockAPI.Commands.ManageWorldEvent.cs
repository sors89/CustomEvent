using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_ManageWorldEvent(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(i => i.OpCode == OpCodes.Br && i.Operand is ILLabel ilLabel && ilLabel.Target == csr.Body.Instructions.Last());

            csr.Emit(OpCodes.Ldarg_0);
            csr.EmitDelegate<Action<TShockAPI.CommandArgs>>(args =>
            {
                args.Player.SendErrorMessage("Valid custom invasion types: {0}.", new object[] { string.Join(", ", Core._validCustomInvasions) });
            });
            csr.Emit(OpCodes.Nop);
        }
    }
}
