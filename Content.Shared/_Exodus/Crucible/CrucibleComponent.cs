using Robust.Shared.Serialization;
using Robust.Shared.GameStates;

namespace Content.Shared._Exodus.Crucible;

[RegisterComponent, NetworkedComponent]
public sealed partial class CrucibleComponent : Component
{
    public static readonly string SlotId = "crucible_slot";
    [ViewVariables] public bool IsProcessing = false;
    [ViewVariables] public TimeSpan FinishTime;
    [ViewVariables] public TimeSpan TotalTime;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class CrucibleRecipeComponent : Component
{
    [DataField("processingTime")] public TimeSpan ProcessingTime = TimeSpan.FromSeconds(30);
    [DataField("resultEntity")] public string ResultEntity = "ItemShipGunPacked";
}

[Serializable, NetSerializable]
public sealed class CrucibleBoundUserInterfaceState : BoundUserInterfaceState
{
    public readonly string? ItemName;
    public readonly bool IsProcessing;
    public readonly TimeSpan FinishTime;
    public readonly TimeSpan TotalTime;
    public readonly bool HasPower;

    public CrucibleBoundUserInterfaceState(string? itemName, bool isProcessing, TimeSpan finishTime, TimeSpan totalTime, bool hasPower)
    {
        ItemName = itemName;
        IsProcessing = isProcessing;
        FinishTime = finishTime;
        TotalTime = totalTime;
        HasPower = hasPower;
    }
}

[Serializable, NetSerializable] public enum CrucibleUiKey : byte { Key }
[Serializable, NetSerializable] public enum CrucibleUiButton : byte { Start, Abort }

[Serializable, NetSerializable]
public sealed class CrucibleUiSendMessage : BoundUserInterfaceMessage
{
    public readonly CrucibleUiButton Button;
    public CrucibleUiSendMessage(CrucibleUiButton button) => Button = button;
}

[Serializable, NetSerializable] public enum CrucibleVisuals : byte { Active }

[Serializable, NetSerializable]
public enum CrucibleLayers : byte { Layer }
