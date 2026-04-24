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
        page.CreateBool("Limits apply to Owner perms", Color.yellow, Globals.ApplyToOwners.entry.Value,
            b => Globals.ApplyToOwners.entry.Value = b);
        page.CreateInt("Max spawns per player", Color.yellow, Globals.MaxSpawnsPerPlayer.entry.Value, 5, 5, 4995,
            i => Globals.MaxSpawnsPerPlayer.entry.Value = i);
    }
}