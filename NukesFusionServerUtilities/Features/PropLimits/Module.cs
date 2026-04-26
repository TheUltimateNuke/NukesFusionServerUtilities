using LabFusion.Entities;
using LabFusion.Network;
using LabFusion.Player;
using LabFusion.RPC;
using LabFusion.SDK.Metadata;
using LabFusion.SDK.Modules;
using LabFusion.Utilities;

namespace NukesFusionServerUtilities.Features.PropLimits;

public class Module : LabFusion.SDK.Modules.Module
{
    public override string Name => "PropLimits";
    public override string Author => "The_UltimateNuke";

    protected override void OnModuleRegistered()
    {
        base.OnModuleRegistered();

        ModuleMessageManager.RegisterHandler<SpawnLimitNotificationMessage>();

        MultiplayerHooking.OnPlayerJoined += MultiplayerHookingOnPlayerJoined;
        MultiplayerHooking.OnPlayerLeft += MultiplayerHookingOnPlayerLeft;
        MultiplayerHooking.OnUpdate += MultiplayerHookingOnOnUpdate;
    }

    protected override void OnModuleUnregistered()
    {
        base.OnModuleUnregistered();

        MultiplayerHooking.OnPlayerJoined -= MultiplayerHookingOnPlayerJoined;
        MultiplayerHooking.OnPlayerLeft -= MultiplayerHookingOnPlayerLeft;
        MultiplayerHooking.OnUpdate -= MultiplayerHookingOnOnUpdate;
    }

    private void MultiplayerHookingOnOnUpdate()
    {
        if (!NetworkInfo.IsHost) return;

        foreach (var playerId in PlayerIDManager.PlayerIDs)
        {
            var netEnts = NetworkEntityManager.IDManager.RegisteredEntities.IDEntityLookup.Values.Where(ne =>
                ne.Source == EntitySource.Player &&
                ne.GetExtender<SpawnerIdentificationExtender>()?.SpawnerId == playerId);
            var props = netEnts.Select(ne => ne.GetExtender<NetworkProp>())
                .Where(np => np.MarrowEntity?.IsDespawned == false).ToList();
            var preCount = props.Count;

            while (preCount > Globals.MaxSpawnsPerPlayer.entry.Value)
            {
                var toRemove = props.Last();
                NetworkAssetSpawner.Despawn(new NetworkAssetSpawner.DespawnRequestInfo
                {
                    DespawnEffect = false,
                    EntityID = toRemove.NetworkEntity.ID
                });
                props.Remove(toRemove);
                preCount = props.Count;
            }

            playerId.Metadata.Metadata.TrySetMetadata(Globals.SpawnsTrackerKey, props.Count.ToString());
        }
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