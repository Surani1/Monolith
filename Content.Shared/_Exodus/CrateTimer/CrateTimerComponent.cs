using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.GameStates;

namespace Content.Shared._Exodus.CrateTimer;

[RegisterComponent, NetworkedComponent]
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

    [DataField("startSound"), ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? StartSound;

    [DataField("loopSound"), ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? LoopSound;

    [DataField("finishSound"), ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier? FinishSound;

    [DataField("loopVolume"), ViewVariables(VVAccess.ReadWrite)]
    public float LoopVolume = -5f;

    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid? LoopStream;

    [ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan NextLoopTime;

    [DataField("loopDelay"), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan LoopDelay = TimeSpan.FromSeconds(1);
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
