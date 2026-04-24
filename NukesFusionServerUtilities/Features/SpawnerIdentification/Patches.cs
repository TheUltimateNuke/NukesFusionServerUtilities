using HarmonyLib;
using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.Player;
using MelonLoader;

namespace NukesFusionServerUtilities.Features.SpawnerIdentification;

[HarmonyPatch]
internal static class Patches
{
    private static void SetOwnerReplacement(NetworkEntity __instance, PlayerID ownerId)
    {
        __instance.SetOwner(ownerId);
        if (__instance.Source != EntitySource.Player) return;

        var spawnExtender = new SpawnerIdentificationExtender(ownerId);
        __instance.ConnectExtender(spawnExtender);

#if DEBUG
        var playerName = ownerId.TryGetDisplayName(out var name) ? name : "UNKNOWN";
        Melon<Entrypoint>.Logger.Msg(ConsoleColor.Gray,
            $"SpawnerId set to player \"{playerName}\". ID: {ownerId.SmallID}");
#endif
    }

    [HarmonyPatch(typeof(SpawnResponseMessage), "CreateNetworkEntity")]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CreateNetworkEntityTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.MethodReplacer(AccessTools.Method(typeof(NetworkEntity), nameof(NetworkEntity.SetOwner)),
            AccessTools.Method(typeof(Patches), nameof(SetOwnerReplacement)));
    }
}