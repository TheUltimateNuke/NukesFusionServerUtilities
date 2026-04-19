using BoneLib.BoneMenu;
using UnityEngine;

namespace NukesFusionServerUtilities.Features;

public abstract class Feature
{
    protected virtual void SetupConfiguration(ref Page? page)
    {
        page = BoneLib.BoneMenu.Page.Root.CreatePage("Nuke's Fusion Server Utilities", Color.yellow);
    }

    public virtual void Initialize()
    {
        Page? page = null;
        SetupConfiguration(ref page);
    }
    public abstract void Deinitialize();
}