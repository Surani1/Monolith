using Content.Shared.CrateTimer;
using Content.Shared.Interaction;
using Content.Server.Chat.Systems;
using Robust.Shared.Timing;
using Robust.Server.GameObjects;

namespace Content.Server.CrateTimer;

public sealed class TimerCrateSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TimerCrateComponent, ActivateInWorldEvent>(OnInteract);
        SubscribeLocalEvent<TimerCrateComponent, TimerCrateActivateMessage>(OnUiActivate);
    }

    private void OnInteract(EntityUid uid, TimerCrateComponent component, ActivateInWorldEvent args)
    {
        _ui.OpenUi(uid, TimerCrateUiKey.Key, args.User);
        UpdateUI(uid, component);
    }

    private void OnUiActivate(EntityUid uid, TimerCrateComponent component, TimerCrateActivateMessage args)
    {
        if (component.State != TimerCrateState.Idle) return;

        component.State = TimerCrateState.Activating;
        component.NextEventTime = _timing.CurTime + component.ActivationDelay;

        _chat.DispatchGlobalAnnouncement(
            "Внимание! Запущен таймер вызова груза Синдиката.",
            "Диспетчер",
            colorOverride: Color.Cyan);

        UpdateAppearance(uid, component);
        UpdateUI(uid, component);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<TimerCrateComponent>();
        while (query.MoveNext(out var uid, out var timer))
        {
            if (timer.State == TimerCrateState.Idle || _timing.CurTime < timer.NextEventTime)
                continue;

            if (timer.State == TimerCrateState.Activating)
            {
                EntityManager.SpawnEntity(timer.CratePrototype, Transform(uid).Coordinates);

                timer.State = TimerCrateState.Cooldown;
                timer.NextEventTime = _timing.CurTime + timer.CooldownDelay;

                _chat.DispatchGlobalAnnouncement("Груз доставлен. Маяк ушел на перезарядку.", "Диспетчер");
            }
            else
            {
                timer.State = TimerCrateState.Idle;
            }

            UpdateAppearance(uid, timer);
            UpdateUI(uid, timer);
        }
    }

    private void UpdateUI(EntityUid uid, TimerCrateComponent comp)
    {
        var total = comp.State == TimerCrateState.Activating ? comp.ActivationDelay : comp.CooldownDelay;
        _ui.SetUiState(uid, TimerCrateUiKey.Key, new TimerCrateBoundUserInterfaceState(comp.State, comp.NextEventTime, total));
    }

    private void UpdateAppearance(EntityUid uid, TimerCrateComponent comp)
    {
        _appearance.SetData(uid, TimerCrateVisuals.State, comp.State);
    }
}
