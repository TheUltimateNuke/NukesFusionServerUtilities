using System.Runtime.InteropServices;
using LabFusion.Network;
using LabFusion.Network.Serialization;
using LabFusion.Player;
using LabFusion.SDK.Modules;
using LabFusion.UI.Popups;

namespace NukesFusionServerUtilities.Features.PropLimits;

public class SpawnLimitNotificationMessageData : INetSerializable
{
    public int Limit;

    public int? GetSize()
    {
        return Marshal.SizeOf(Limit);
    }

    public void Serialize(INetSerializer serializer)
    {
        serializer.SerializeValue(ref Limit);
    }
}

public class SpawnLimitNotificationMessage : ModuleMessageHandler
{
    public override ExpectedReceiverType ExpectedReceiver => ExpectedReceiverType.Both;

    protected override void OnHandleMessage(ReceivedMessage received)
    {
        if (received.Sender != PlayerIDManager.GetHostID())
            return;
        var data = received.ReadData<SpawnLimitNotificationMessageData>();

        var notif = new Notification
        {
            Title = "Spawnable Limit Reached!",
            Type = NotificationType.ERROR,
            Message =
                $"Your max spawnable limit of {data.Limit} has been reached! Any exceeding spawnables will not be spawned or replicated for others!",
            ShowPopup = true
        };

        Notifier.Send(notif);
    }
}