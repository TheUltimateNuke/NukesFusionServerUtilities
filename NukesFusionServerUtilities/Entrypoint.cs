using BoneLib.BoneMenu;
using LabFusion.SDK.Modules;
using MelonLoader;
using NukesFusionServerUtilities;
using NukesFusionServerUtilities.Features;
using NukesFusionServerUtilities.Features.PropLimits;
using UnityEngine;
using Module = NukesFusionServerUtilities.Features.PropLimits.Module;

[assembly: MelonInfo(typeof(Entrypoint), MyModInfo.Name, MyModInfo.Version, MyModInfo.Author)]
[assembly: MelonAdditionalDependencies("LabFusion", "BoneLib")]
[assembly: MelonOptionalDependencies("SceneJanitor")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace NukesFusionServerUtilities;

public class Entrypoint : MelonMod
{
    public static readonly MelonPreferences_Category Category =
        MelonPreferences.CreateCategory(nameof(NukesFusionServerUtilities));

    // Static mod accessors for easy cross-class usage
    internal static HarmonyLib.Harmony StaticHarmonyInstance => Melon<Entrypoint>.Instance.HarmonyInstance;
    internal static MelonLogger.Instance Logger => Melon<Entrypoint>.Logger;

    public override void OnInitializeMelon()
    {
        var bonePage = Page.Root.CreatePage(nameof(NukesFusionServerUtilities), Color.green);

        PanicButton.SetupConfiguration(bonePage);
        PanicButton.Initialize();

        BoneMenuCreator.Create(bonePage);
        ModuleManager.RegisterModule<Module>();

        Logger.Msg(System.ConsoleColor.Green, $"{MyModInfo.Name} v{MyModInfo.Version} initialized!");
    }
}