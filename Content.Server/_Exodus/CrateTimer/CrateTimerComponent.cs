using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

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

    // Настройки звуков
    [DataField("startSound"), ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? StartSound = new SoundPathSpecifier("/Audio/Machines/button.ogg");

    [DataField("loopSound"), ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? LoopSound = new SoundPathSpecifier("/Audio/Machines/terminal_processing.ogg");

    [DataField("finishSound"), ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? FinishSound = new SoundPathSpecifier("/Audio/Effects/teleport_arrival.ogg");

    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid? LoopStream;
}

[Serializable, NetSerializable]
public enum TimerCrateState : byte
{
    Idle,
    Activating,
    Cooldown
}

[Serializable, NetSerializable]
public enum TimerCrateVisuals : byte
{
    State
}
