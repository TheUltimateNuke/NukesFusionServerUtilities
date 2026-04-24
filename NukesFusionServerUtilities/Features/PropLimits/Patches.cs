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
        var notif = new Notification
        {
            Title = "Spawnable Limit Reached!",
            Type = NotificationType.ERROR,
            Message =
                $"Max spawnable limit of {limit} has been reached! Any exceeding spawnables will not be spawned or replicated for others!",
            ShowPopup = true
        };

        var exists = Notifier.CurrentNotification?.Title.Text == notif.Title.Text;
        if (!exists) Notifier.Send(notif);
    }

    [HarmonyPatch(typeof(SpawnRequestMessage), "OnHandleMessage")]
    [HarmonyPrefix]
    private static bool OnHandleSpawnMessagePrefix(ReceivedMessage received)
    {
        if (!Globals.Enabled.entry.Value) return true;
        var data = received.ReadData<SerializedSpawnData>();
        if (data.SpawnSource != EntitySource.Player) return true;
        var owner = received.Sender;
        if (owner == null) return true;
        var playerId = PlayerIDManager.GetPlayerID((byte)owner);
        playerId.TryGetPermissionLevel(out var permissionLevel);
        var found = playerId.Metadata.Metadata.TryGetMetadata(Globals.SpawnsTrackerKey, out var count);
        if (((permissionLevel.HasFlag(PermissionLevel.OWNER) || permissionLevel.HasFlag(PermissionLevel.OPERATOR)) &&
             !Globals.ApplyToOperators.entry.Value) || !found ||
            int.Parse(count) < Globals.MaxSpawnsPerPlayer.entry.Value)
            return true;
#if DEBUG
        Melon<Entrypoint>.Logger.Warning(
            $"Blocking server spawn of {data.Barcode} because sender exceeded max spawn count of {Globals.MaxSpawnsPerPlayer.entry.Value}!");
#endif
        OnSpawnLimitReached(Globals.MaxSpawnsPerPlayer.entry.Value);
        return false;
    }

    [HarmonyPatch(typeof(DespawnRequestMessage), "OnHandleMessage")]
    [HarmonyPrefix]
    private static bool OnHandleDespawnMessagePrefix(ReceivedMessage received)
    {
        if (!Globals.Enabled.entry.Value) return true;
        var owner = received.Sender;
        if (owner is null) return true;
        var ownerId = PlayerIDManager.GetPlayerID((byte)owner);
        ownerId.TryGetPermissionLevel(out var permissionLevel);
        var data = received.ReadData<DespawnRequestData>();
        var ent = data.Entity.GetEntity();
        var ext = ent?.GetExtender<SpawnerIdentificationExtender>();
        return ext?.SpawnerId == ownerId || Globals.DespawnOthersSpawnables.entry.Value ||
               ((permissionLevel.HasFlag(PermissionLevel.OWNER) || permissionLevel.HasFlag(PermissionLevel.OPERATOR)) &&
                Globals.DespawnOthersSpawnablesIfOp.entry.Value);
    }

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