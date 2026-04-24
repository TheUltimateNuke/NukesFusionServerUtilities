using BoneLib;
using LabFusion.SDK.Metadata;

namespace NukesFusionServerUtilities.Features.PropLimits;

public static class Globals
{
    public const string SpawnsTrackerKey = $"{nameof(PropLimits)}.{nameof(SpawnsDict)}";
    public static readonly ModPref<bool> Enabled = new(Entrypoint.Category, nameof(Enabled), true);
    public static readonly ModPref<int> MaxSpawnsPerPlayer = new(Entrypoint.Category, nameof(MaxSpawnsPerPlayer), 15);
    public static readonly ModPref<bool> ApplyToOwners = new(Entrypoint.Category, nameof(ApplyToOwners), false);
    public static MetadataInt? SpawnsDict { get; set; }
}