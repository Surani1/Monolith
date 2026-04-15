using Content.Shared.Interaction;
using Content.Server.Chat.Systems;
using Content.Shared.Popups;
using Robust.Shared.Timing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Server.GameObjects;

namespace Content.Server._Exodus.CrateTimer;

public sealed class TimerCrateSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly EntityManager _entManager = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly AppearanceSystem _appearance = default!;
    [Dependency] private readonly PointLightSystem _light = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TimerCrateComponent, ActivateInWorldEvent>(OnActivate);
    }

    private void OnActivate(EntityUid uid, TimerCrateComponent component, ActivateInWorldEvent args)
    {
        if (args.Handled) return;

        switch (component.State)
        {
            case TimerCrateState.Idle:
                StartTimer(uid, component);
                _popup.PopupEntity(Loc.GetString("timer-crate-startup"), uid, args.User);
                args.Handled = true;
                break;
            case TimerCrateState.Activating:
                var remaining = component.NextEventTime - _timing.CurTime;
                _popup.PopupEntity(Loc.GetString("timer-crate-status-remaining",
                    ("min", remaining.Minutes), ("sec", remaining.Seconds)), uid, args.User);
                break;
            case TimerCrateState.Cooldown:
                _popup.PopupEntity(Loc.GetString("timer-crate-status-cooldown"), uid, args.User);
                break;
        }
    }

    private void StartTimer(EntityUid uid, TimerCrateComponent component)
    {
        component.State = TimerCrateState.Activating;
        component.NextEventTime = _timing.CurTime + component.ActivationDelay;

        UpdateVisuals(uid, component);

        _audio.PlayPvs(component.StartSound, uid);

        if (component.LoopSound != null)
        {
            component.LoopStream = _audio.PlayPvs(component.LoopSound, uid,
                AudioParams.Default.WithLoop(true).WithVolume(-4f))?.Entity;
        }

        _chat.DispatchGlobalAnnouncement(
            Loc.GetString("timer-crate-announcement-start"),
            Loc.GetString("timer-crate-announcement-sender"),
            colorOverride: Color.FromHex("#34ebcf"));
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<TimerCrateComponent>();
        while (query.MoveNext(out var uid, out var timer))
        {
            if (timer.State == TimerCrateState.Idle || _timing.CurTime < timer.NextEventTime)
                continue;

            ProcessStateTransition(uid, timer);
        }
    }

    private void ProcessStateTransition(EntityUid uid, TimerCrateComponent timer)
    {
        if (timer.State == TimerCrateState.Activating)
        {
            timer.LoopStream = _audio.Stop(timer.LoopStream);
            _audio.PlayPvs(timer.FinishSound, uid);

            _entManager.SpawnEntity(timer.CratePrototype, Transform(uid).Coordinates);

            timer.State = TimerCrateState.Cooldown;
            timer.NextEventTime = _timing.CurTime + timer.CooldownDelay;

            _chat.DispatchGlobalAnnouncement(
                Loc.GetString("timer-crate-announcement-finish"),
                Loc.GetString("timer-crate-announcement-sender"));
        }
        else if (timer.State == TimerCrateState.Cooldown)
        {
            timer.State = TimerCrateState.Idle;
        }

        UpdateVisuals(uid, timer);
    }

    private void UpdateVisuals(EntityUid uid, TimerCrateComponent component)
    {
        _appearance.SetData(uid, TimerCrateVisuals.State, component.State);

        if (TryComp<PointLightComponent>(uid, out var light))
        {
            _light.SetEnabled(uid, component.State == TimerCrateState.Activating, light);
        }
    }
}
