using Robust.Shared.Audio;

namespace Content.Server.CrateTimer;

[RegisterComponent] // НЕТ NetSerializable!
public sealed partial class TimerCrateAudioComponent : Component
{
    [DataField] public SoundSpecifier ClickSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg");
    [DataField] public SoundSpecifier ActiveSound = new SoundPathSpecifier("/Audio/Ambience/Objects/terminal_hum.ogg");
    [DataField] public SoundSpecifier FinishSound = new SoundPathSpecifier("/Audio/Effects/teleport_arrival.ogg");

    public EntityUid? Stream;
}
