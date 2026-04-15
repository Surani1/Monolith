using Robust.Shared.Serialization;

namespace Content.Shared._Exodus.CrateTimer;
[Serializable, NetSerializable]
public enum TimerCrateVisuals : byte
{
    State
}

[Serializable, NetSerializable]
public enum TimerCrateState : byte
{
    Idle,
    Activating,
    Cooldown
}
