using Content.Shared.CrateTimer;
using Robust.Client.GameObjects;

namespace Content.Client.CrateTimer;

public sealed class TimerCrateSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TimerCrateComponent, AppearanceChangeEvent>(OnAppearanceChange);
    }

    private void OnAppearanceChange(EntityUid uid, TimerCrateComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        // Используем систему AppearanceSystem для получения данных состояния
        if (!_appearance.TryGetData<TimerCrateState>(uid, TimerCrateVisuals.State, out var state, args.Component))
            return;

        // Определяем имя стейта спрайта
        var stateName = state == TimerCrateState.Activating ? component.OnState : component.OffState;

        // Устанавливаем состояние спрайта
        args.Sprite.LayerSetState(0, stateName);
    }
}
