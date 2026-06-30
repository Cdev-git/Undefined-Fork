using HarmonyLib;
using JetBrains.Annotations;
using PlayFab.EventsModels;

namespace Undefined.Patches;

public class TelemetryPatches
{
    public static bool enabled = true;

    [HarmonyPatch(typeof(GorillaTelemetry), "EnqueueTelemetryEvent")]
    public class TelemetryPatch1
    {
        private static bool Prefix(string eventName, object content, [CanBeNull] string[] customTags = null) =>
            !enabled;
    }
}
