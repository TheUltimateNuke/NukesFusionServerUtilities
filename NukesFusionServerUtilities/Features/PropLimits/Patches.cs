using HarmonyLib;
using LabFusion.Entities;
using LabFusion.Marrow.Serialization;
using LabFusion.Network;
using LabFusion.Player;
using LabFusion.Representation;
using LabFusion.RPC;
using LabFusion.UI.Popups;
using MelonLoader;

namespace NukesFusionServerUtilities.Features.PropLimits;

[HarmonyPatch]
internal static class Patches
{
    [Rpc(RelayType.ToTarget)]
    private static void OnSpawnLimitReached(int limit)
    {
        Notifier.Send(new Notification
        {
            Title = "Spawnable Limit Reached!",
            Type = NotificationType.ERROR,
            Message =
                $"Max spawnable limit of {limit} has been reached! Any exceeding spawnables will not be spawned or replicated for others!",
            ShowPopup = true
        });
    }

    [HarmonyPatch(typeof(SpawnRequestMessage), "OnHandleMessage")]
    [HarmonyPrefix]
    private static bool OnHandleMessagePrefix(SpawnRequestMessage __instance, ReceivedMessage received)
    {
        var data = received.ReadData<SerializedSpawnData>();
        if (!Globals.Enabled.entry.Value) return true;
        if (data.SpawnSource != EntitySource.Player) return true;
        var owner = received.Sender;
        if (owner == null) return true;
        var playerId = PlayerIDManager.GetPlayerID((byte)owner);
        playerId.TryGetPermissionLevel(out var permissionLevel);
        var found = playerId.Metadata.Metadata.TryGetMetadata(Globals.SpawnsTrackerKey, out var count);
        if ((permissionLevel.HasFlag(PermissionLevel.OWNER) && !Globals.ApplyToOwners.entry.Value) || !found ||
            int.Parse(count) + 1 < Globals.MaxSpawnsPerPlayer.entry.Value)
            return true;
#if DEBUG
        Melon<Entrypoint>.Logger.Warning(
            $"Blocking server spawn of {data.Barcode} because sender exceeded max spawn count of {Globals.MaxSpawnsPerPlayer.entry.Value}!");
#endif
        OnSpawnLimitReached(Globals.MaxSpawnsPerPlayer.entry.Value);
        return false;
    }
}