using System.Reflection;
using BoneLib;
using BoneLib.BoneMenu;
using BoneLib.Notifications;
using LabFusion.Network;
using LabFusion.Player;
using MelonLoader;
using UnityEngine;

namespace NukesFusionServerUtilities.Features;

/// <summary>
/// Spam secondary input button or B key to reset player, teleport to spawnpoint + if host, clean up the map. Meant to prevent chaotically laggy servers or stuck players in an emergency.
/// </summary>
public class PanicButton : Feature
{
    private const int DefaultTriggerCount = 10;
    
    public static MelonPreferences_Category Category = MelonPreferences.CreateCategory(nameof(NukesFusionServerUtilities));
    public static ModPref<int> PanicButtonTriggerCountPref = new(Category, nameof(PanicButtonTriggerCountPref), DefaultTriggerCount, "Panic Button Trigger Count");
    public static ModPref<bool> PanicButtonResetAvatarPref = new(Category, nameof(PanicButtonResetAvatarPref), false, "Fallback Avatar on Panic Button");
    
    private static BindingFlags _allFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    
    private static float _secondsBetweenInputs = 0.8f;
    private static int _count;
    private static float _lastIncTime;

    public override void Initialize()
    {
        base.Initialize();
        
        MelonEvents.OnUpdate.Subscribe(OnUpdate);
    }

    protected override void SetupConfiguration(ref Page page)
    {
        base.SetupConfiguration(ref page);
        
        page.CreateBool("Fallback Avatar on Panic Button", Color.gray, PanicButtonResetAvatarPref.entry.Value, i => PanicButtonResetAvatarPref.entry.Value = i);
        page.CreateInt("Panic Button Trigger Count", Color.gray, PanicButtonTriggerCountPref.entry.Value, 1, 2, 16, i => PanicButtonTriggerCountPref.entry.Value = i);
    }

    public override void Deinitialize()
    {
        MelonEvents.OnUpdate.Unsubscribe(OnUpdate);
    }

    public static void OnTriggerPanicButton()
    {
        if (PanicButtonResetAvatarPref.entry.Value) LocalAvatar.SwapAvatarCrate(CommonBarcodes.Avatars.PolyBlank);
        LocalPlayer.ClearConstraints();
        LocalPlayer.TeleportToCheckpoint();
        if (NetworkInfo.IsHost)
        {
            // clean up scene using Scene Janitor if local player is Fusion host (if Scene Janitor is installed)
            var sceneJanitorMelon = MelonBase.FindMelon("Scene Janitor", "minecart");
            sceneJanitorMelon?.GetType().GetMethod("RestoreCleanup", _allFlags)?.Invoke(sceneJanitorMelon, null);
        }
        
        Notifier.Send(new Notification
        {
            Type = NotificationType.Success,
            PopupLength = 5,
            Message = "Panic button triggered! You have been reset!"
        });
    }

    private void OnUpdate()
    {
        if (_secondsBetweenInputs <= Time.realtimeSinceStartup - _lastIncTime)
            _count = 0; // reset count if input is not spammed in _secondsBetweenInputs seconds
        
        if (Player.LeftController?.GetSecondaryInteractionButtonDown() == true || UnityEngine.InputSystem.Keyboard.current.bKey.wasPressedThisFrame)
        {
            _count++;
            _lastIncTime = Time.realtimeSinceStartup;
        }

        if (_count < PanicButtonTriggerCountPref.entry.Value) return;
        _count = 0;
        OnTriggerPanicButton();
    }
}