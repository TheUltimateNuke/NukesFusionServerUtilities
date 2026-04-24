using BoneLib.BoneMenu;
using UnityEngine;

namespace NukesFusionServerUtilities.Features.PropLimits;

internal static class BoneMenuCreator
{
    internal static void Create(Page modPage)
    {
        var page = modPage.CreatePage(nameof(PropLimits), Color.yellow);

        page.CreateBool("Enabled", Color.yellow, Globals.Enabled.entry.Value,
            b => { Globals.Enabled.entry.Value = b; });
        page.CreateBool("Allow despawn of others' spawns", Color.yellow, Globals.DespawnOthersSpawnables.entry.Value,
            b => { Globals.DespawnOthersSpawnables.entry.Value = b; });
        page.CreateBool("Allow despawn of others' spawns (Admin only)", Color.yellow,
            Globals.DespawnOthersSpawnablesIfOp.entry.Value,
            b => { Globals.DespawnOthersSpawnablesIfOp.entry.Value = b; });
        page.CreateBool("Spawn limits apply to Operator perms or above", Color.yellow,
            Globals.ApplyToOperators.entry.Value,
            b => { Globals.ApplyToOperators.entry.Value = b; });
        page.CreateInt("Max spawns per player", Color.yellow, Globals.MaxSpawnsPerPlayer.entry.Value, 10, 5, 500,
            i => { Globals.MaxSpawnsPerPlayer.entry.Value = i; });
    }
}