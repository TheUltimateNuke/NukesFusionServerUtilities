using NukesFusionServerUtilities;
using MelonLoader;
using NukesFusionServerUtilities.Features;

[assembly: MelonInfo(typeof(Entrypoint), MyModInfo.Name, MyModInfo.Version, MyModInfo.Author)]
[assembly: MelonOptionalDependencies("SceneJanitor")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace NukesFusionServerUtilities;

public class Entrypoint : MelonMod
{
    // Static mod accessors for easy cross-class usage
    internal static HarmonyLib.Harmony StaticHarmonyInstance => Melon<Entrypoint>.Instance.HarmonyInstance;
    internal static MelonLogger.Instance Logger => Melon<Entrypoint>.Logger;

    public override void OnInitializeMelon()
    {
        new PanicButton().Initialize(); // TODO: Make FeatureManager
        
        Logger.Msg(System.ConsoleColor.Green, $"{MyModInfo.Name} v{MyModInfo.Version} initialized!");
    }
}