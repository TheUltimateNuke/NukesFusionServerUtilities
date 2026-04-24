using System.Reflection;
using BoneLib;
using BoneLib.BoneMenu;
using BoneLib.Notifications;
using LabFusion.Network;
using LabFusion.Player;
using LabFusion.Scene;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NukesFusionServerUtilities.Features;

/// <summary>
/// Spam secondary input button or B key to reset player, teleport to spawnpoint + if host, clean up the map. Meant to prevent chaotically laggy servers or stuck players in an emergency.
/// </summary>
public static class PanicButton
{
    public enum CleanupMethod
    {
        None = 0,
        SceneJanitor = 1,
        LevelReload = 2
    }

    private const int DefaultTriggerCount = 10;

    private static readonly ModPref<int> _panicButtonTriggerCountPref = new(Entrypoint.Category,
        nameof(_panicButtonTriggerCountPref), DefaultTriggerCount, "Panic Button Trigger Count");

    private static readonly ModPref<bool> _panicButtonResetAvatarPref = new(Entrypoint.Category,
        nameof(_panicButtonResetAvatarPref), false, "Fallback Avatar on Panic Button");

    private static readonly ModPref<CleanupMethod> _cleanupMethodPref = new(Entrypoint.Category,
        nameof(_cleanupMethodPref),
        CleanupMethod.SceneJanitor, "Use SceneJanitor");

    private static readonly BindingFlags _allFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private static float _secondsBetweenInputs = 0.8f;
    private static int _count;
    private static float _lastIncTime;

    internal static void Initialize()
    {
        MelonEvents.OnUpdate.Subscribe(OnUpdate);
    }

    internal static void SetupConfiguration(Page modPage)
    {
        var panicBtnPage = modPage.CreatePage(nameof(PanicButton), Color.red);
        panicBtnPage.CreateBool("Fallback avatar on panic button", Color.gray, _panicButtonResetAvatarPref.entry.Value,
            i => _panicButtonResetAvatarPref.entry.Value = i);
        panicBtnPage.CreateEnum("Cleanup on panic button method", Color.gray, _cleanupMethodPref.entry.Value,
            i => _cleanupMethodPref.entry.Value = (CleanupMethod)i);
        panicBtnPage.CreateInt("Panic button trigger count", Color.gray, _panicButtonTriggerCountPref.entry.Value, 1, 2,
            16, i => _panicButtonTriggerCountPref.entry.Value = i);
    }

    public static void OnTriggerPanicButton()
    {
        if (_panicButtonResetAvatarPref.entry.Value) LocalAvatar.SwapAvatarCrate(CommonBarcodes.Avatars.PolyBlank);
        LocalPlayer.ClearConstraints();
        LocalPlayer.TeleportToCheckpoint();
        if (NetworkInfo.IsHost)
        {
            switch (_cleanupMethodPref.entry.Value)
            {
                case CleanupMethod.SceneJanitor:
                    // clean up scene using Scene Janitor if local player is Fusion host (if Scene Janitor is installed)
                    var sceneJanitorMelon = MelonBase.FindMelon("Scene Janitor", "minecart");
                    sceneJanitorMelon?.GetType().GetMethod("RestoreCleanup", _allFlags)
                        ?.Invoke(sceneJanitorMelon, null);
                    break;
                case CleanupMethod.LevelReload:
                    FusionSceneManager.LoadTargetScene();
                    break;
            }
        }

        Notifier.Send(new Notification
        {
            Type = NotificationType.Success,
            PopupLength = 2,
            Message = "Panic button triggered! You have been reset!"
        });
    }

    private static void OnUpdate()
    {
        if (_secondsBetweenInputs <= Time.realtimeSinceStartup - _lastIncTime)
            _count = 0; // reset count if input is not spammed in _secondsBetweenInputs seconds

        if (Player.LeftController?.GetBButtonDown() == true || Keyboard.current?.bKey?.wasPressedThisFrame == true)
        {
            _count++;
            _lastIncTime = Time.realtimeSinceStartup;
        }

        if (_count < _panicButtonTriggerCountPref.entry.Value) return;
        _count = 0;
        OnTriggerPanicButton();
    }
}