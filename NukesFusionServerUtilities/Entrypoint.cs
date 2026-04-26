using BoneLib;
using LabFusion.Network;
using LabFusion.Scene;
using LabFusion.SDK.Modules;
using MelonLoader;
using NukesFusionServerUtilities;
using NukesFusionServerUtilities.Features;
using NukesFusionServerUtilities.Features.PropLimits;
using UnityEngine;
using Module = NukesFusionServerUtilities.Features.PropLimits.Module;
using Page = BoneLib.BoneMenu.Page;

[assembly: MelonInfo(typeof(Entrypoint), MyModInfo.Name, MyModInfo.Version, MyModInfo.Author)]
[assembly: MelonAdditionalDependencies("LabFusion", "BoneLib")]
[assembly: MelonOptionalDependencies("SceneJanitor")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace NukesFusionServerUtilities;

public class Entrypoint : MelonMod
{
    public static readonly MelonPreferences_Category Category =
        MelonPreferences.CreateCategory(nameof(NukesFusionServerUtilities));

    internal static ModPref<bool> StartServerOnLevelLoad = new(Category, nameof(StartServerOnLevelLoad), false);

    // Static mod accessors for easy cross-class usage
    internal static MelonLogger.Instance Logger => Melon<Entrypoint>.Logger;

    public override void OnInitializeMelon()
    {
        var bonePage = Page.Root.CreatePage(nameof(NukesFusionServerUtilities), Color.green);

        bonePage.CreateBool("Start Fusion server on level load", Color.cyan, StartServerOnLevelLoad.entry.Value,
            b => StartServerOnLevelLoad.entry.Value = b);

        PanicButton.SetupConfiguration(bonePage);
        PanicButton.Initialize();

        BoneMenuCreator.Create(bonePage);
        ModuleManager.RegisterModule<Module>();

        StartFusionServerOnLevelLoad();

        Logger.Msg(System.ConsoleColor.Green, $"{MyModInfo.Name} v{MyModInfo.Version} initialized!");
    }

    public static void StartFusionServerOnLevelLoad()
    {
        FusionSceneManager.HookOnDelayedLevelLoad(() =>
        {
            if (!StartServerOnLevelLoad.entry.Value) return;
            if (NetworkInfo.HasServer) return;
            if (!NetworkInfo.HasLayer)
            {
                var netLayer = NetworkLayer.GetLayer<SteamVRNetworkLayer>();
                netLayer.LogIn();
            }

            NetworkHelper.StartServer();
        });
    }
}