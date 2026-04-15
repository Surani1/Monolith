using Robust.Shared.Serialization;
using Robust.Shared.Prototypes;

namespace Content.Shared.CrateTimer;

[RegisterComponent, NetSerializable]
public sealed partial class TimerCrateComponent : Component
{
    [DataField] public TimeSpan ActivationDelay = TimeSpan.FromMinutes(15);
    [DataField] public TimeSpan CooldownDelay = TimeSpan.FromMinutes(45);
    [DataField] public EntProtoId CratePrototype = "CrateSyndicateSurplusBundle";

    [DataField] public string OffState = "borgcharger-u1";
    [DataField] public string OnState = "borgcharger-u0";

    public TimerCrateState State = TimerCrateState.Idle;
    public TimeSpan NextEventTime;
}

[NetSerializable]
public enum TimerCrateState : byte { Idle, Activating, Cooldown }
[NetSerializable]
public enum TimerCrateUiKey : byte { Key }
[NetSerializable]
public enum TimerCrateVisuals : byte { State }

[NetSerializable]
public sealed class TimerCrateBoundUserInterfaceState : BoundUserInterfaceState
{
    public TimerCrateState CurrentState;
    public TimeSpan NextEventTime;
    public TimeSpan TotalDuration;

    public TimerCrateBoundUserInterfaceState(TimerCrateState currentState, TimeSpan nextEventTime, TimeSpan totalDuration)
    {
        CurrentState = currentState;
        NextEventTime = nextEventTime;
        TotalDuration = totalDuration;
    }
}

[NetSerializable]
public sealed class TimerCrateActivateMessage : BoundUserInterfaceMessage { }
