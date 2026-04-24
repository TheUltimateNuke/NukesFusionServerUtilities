using BoneLib;
using LabFusion.SDK.Metadata;

namespace NukesFusionServerUtilities.Features.PropLimits;

public static class Globals
{
    public const string SpawnsTrackerKey = $"{nameof(PropLimits)}.{nameof(SpawnsDict)}";
    public static readonly ModPref<bool> Enabled = new(Entrypoint.Category, nameof(Enabled), true);

    public static readonly ModPref<bool> DespawnOthersSpawnables =
        new(Entrypoint.Category, nameof(DespawnOthersSpawnables), true);

    public static readonly ModPref<bool> DespawnOthersSpawnablesIfOp =
        new(Entrypoint.Category, nameof(DespawnOthersSpawnablesIfOp), true);

    public static readonly ModPref<int> MaxSpawnsPerPlayer = new(Entrypoint.Category, nameof(MaxSpawnsPerPlayer), 15);
    public static readonly ModPref<bool> ApplyToOperators = new(Entrypoint.Category, nameof(ApplyToOperators), false);
    public static MetadataInt? SpawnsDict { get; set; }
}