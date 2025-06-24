using System.Reflection;

namespace CustomEvent.Patches
{
    internal static class CachedFields
    {
        //Vanilla invasion fields.
        internal static readonly FieldInfo invasionType = typeof(Terraria.Main).GetField("invasionType")!;
        internal static readonly FieldInfo invasionSize = typeof(Terraria.Main).GetField("invasionSize")!;
        internal static readonly FieldInfo invasionDelay = typeof(Terraria.Main).GetField("invasionDelay")!;

        //Custom invasion event fields.
        internal static readonly FieldInfo eventType = typeof(CustomEvent.Core).GetField("eventType")!;
        internal static readonly FieldInfo eventSize = typeof(CustomEvent.Modules.InvasionCore).GetField("eventSize")!;
    }
}
