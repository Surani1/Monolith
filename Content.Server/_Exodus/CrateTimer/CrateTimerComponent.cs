using Robust.Shared.Prototypes;

namespace Content.Server._Exodus.CrateTimer;

[RegisterComponent]
public sealed partial class TimerCrateComponent : Component
{
    [DataField("activationDelay"), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan ActivationDelay = TimeSpan.FromMinutes(15);

    [DataField("cooldownDelay"), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan CooldownDelay = TimeSpan.FromMinutes(45);

    [DataField("cratePrototype"), ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId CratePrototype = "CrateSyndicateSurplusBundle";

    [ViewVariables(VVAccess.ReadWrite)]
    public TimerCrateState State = TimerCrateState.Idle;

    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan NextEventTime;
}

public enum TimerCrateState : byte
{
    Idle,
    Activating,
    Cooldown
}
