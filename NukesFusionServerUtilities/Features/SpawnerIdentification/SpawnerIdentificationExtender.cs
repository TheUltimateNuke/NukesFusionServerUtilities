using LabFusion.Entities;

namespace NukesFusionServerUtilities.Features.SpawnerIdentification;

public class SpawnerIdentificationExtender(byte spawnerId) : IEntityExtender
{
    public byte SpawnerId => spawnerId;
}