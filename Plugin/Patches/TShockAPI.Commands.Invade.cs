using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace CustomEvent.Patches
{
    internal static partial class Patch
    {
        internal static void ILHook_Invade(ILContext ctx)
        {
            var csr = new ILCursor(ctx);
            csr.GotoNext(i => i.OpCode == OpCodes.Br && i.Operand is ILLabel ilLabel && ilLabel.Target == csr.Body.Instructions.Last());

            csr.Emit(OpCodes.Ldarg_0);
            csr.EmitDelegate<Action<TShockAPI.CommandArgs>>(args =>
            {
                args.Player.SendErrorMessage("Valid custom invasion types: {0}.", new object[] { string.Join(", ", Core._validCustomInvasions) });
            });
            csr.Emit(OpCodes.Nop);

            while (csr.TryGotoNext(MoveType.After, i => i.MatchCallvirt(typeof(TShockAPI.TSPlayer), "SendErrorMessage"),
                                                   i => i.MatchNop())) ;

            csr.Emit(OpCodes.Ldarg_0);
            csr.EmitDelegate<Action<TShockAPI.CommandArgs>>(args =>
            {
                args.Player.SendErrorMessage("Valid custom invasion types: {0}.", new object[] { string.Join(", ", Core._validCustomInvasions) });
            });
            csr.Emit(OpCodes.Nop);
        }

        internal static void MMHook_Invade(Action<TShockAPI.CommandArgs> orig, TShockAPI.CommandArgs args)
        {
            if (CustomEvent.Modules.InvasionCore.eventSize <= 0)
            {
                if (args.Parameters.Count == 2)
                {
                    string evName = args.Parameters[1].ToLowerInvariant();
                    foreach (var evins in Core.eventInstances)
                    {
                        if (evins.Alias.Contains(evName))
                        {
                            TShockAPI.TSPlayer.All.SendInfoMessage("{0} has started {1}!", args.Player.Name, evins.EventName);
                            Terraria.Main.StartInvasion(evins.EventID);
                            return;
                        }
                    }
                }
            }
            else
            {
                TShockAPI.TSPlayer.All.SendInfoMessage("{0} has ended the current invasion event.", args.Player.Name);
                CustomEvent.Modules.InvasionCore.eventSize = 0;
                return;
            }
            orig(args);
        }
    }
}
