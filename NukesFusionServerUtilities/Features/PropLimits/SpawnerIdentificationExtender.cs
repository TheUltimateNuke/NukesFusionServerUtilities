using LabFusion.Entities;

namespace NukesFusionServerUtilities.Features.PropLimits;

public class SpawnerIdentificationExtender(byte spawnerId) : IEntityExtender
{
    public byte SpawnerId => spawnerId;
}