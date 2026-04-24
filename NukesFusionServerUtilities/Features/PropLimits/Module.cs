using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.Player;
using LabFusion.SDK.Metadata;
using LabFusion.Utilities;
using NukesFusionServerUtilities.Features.SpawnerIdentification;

namespace NukesFusionServerUtilities.Features.PropLimits;

public class Module : LabFusion.SDK.Modules.Module
{
    public override string Name => "PropLimits";
    public override string Author => "TheUltimateNuke";

    protected override void OnModuleRegistered()
    {
        base.OnModuleRegistered();

        MultiplayerHooking.OnPlayerJoined += MultiplayerHookingOnPlayerJoined;
        MultiplayerHooking.OnPlayerLeft += MultiplayerHookingOnPlayerLeft;
        NetworkEntityManager.IDManager.OnEntityRegistered += IDManagerOnOnEntityRegistered;
        NetworkEntityManager.IDManager.OnEntityUnregistered += IDManagerOnOnEntityUnregistered;
    }

    protected override void OnModuleUnregistered()
    {
        base.OnModuleUnregistered();

        MultiplayerHooking.OnPlayerJoined -= MultiplayerHookingOnPlayerJoined;
        MultiplayerHooking.OnPlayerLeft -= MultiplayerHookingOnPlayerLeft;
        NetworkEntityManager.IDManager.OnEntityRegistered -= IDManagerOnOnEntityRegistered;
        NetworkEntityManager.IDManager.OnEntityUnregistered -= IDManagerOnOnEntityUnregistered;
    }

    private void IDManagerOnOnEntityRegistered(NetworkEntity obj)
    {
        if (!NetworkInfo.IsHost) return;

        var spawnerIdExtender = obj.GetExtender<SpawnerIdentificationExtender>();
        if (spawnerIdExtender == null) return;

        var id = PlayerIDManager.GetPlayerID(spawnerIdExtender.SpawnerId);
        if (id == null) return;

        var metadata = id.Metadata.Metadata;
        metadata.TryGetMetadata(Globals.SpawnsTrackerKey, out var count);
        metadata.TrySetMetadata(Globals.SpawnsTrackerKey,
            !string.IsNullOrWhiteSpace(count) ? (int.Parse(count) + 1).ToString() : "1");
    }

    private void IDManagerOnOnEntityUnregistered(NetworkEntity obj)
    {
        if (!NetworkInfo.IsHost) return;

        var spawnerIdExtender = obj.GetExtender<SpawnerIdentificationExtender>();
        if (spawnerIdExtender == null) return;

        var id = PlayerIDManager.GetPlayerID(spawnerIdExtender.SpawnerId);
        if (id == null) return;

        var metadata = id.Metadata.Metadata;
        metadata.TryGetMetadata(Globals.SpawnsTrackerKey, out var count);
        metadata.TrySetMetadata(Globals.SpawnsTrackerKey,
            !string.IsNullOrWhiteSpace(count) && int.Parse(count) - 1 >= 0 ? (int.Parse(count) - 1).ToString() : "0");
    }

    private static void MultiplayerHookingOnPlayerJoined(PlayerID playerId)
    {
        if (!NetworkInfo.IsHost) return;

        Globals.SpawnsDict = new MetadataInt(Globals.SpawnsTrackerKey, playerId.Metadata.Metadata);
    }

    private static void MultiplayerHookingOnPlayerLeft(PlayerID playerId)
    {
        if (!NetworkInfo.IsHost) return;

        Globals.SpawnsDict = null;
    }
}