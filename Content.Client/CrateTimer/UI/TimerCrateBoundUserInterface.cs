using Content.Shared.CrateTimer;
using Robust.Client.GameObjects;

using Content.Client.CrateTimer.UI;

namespace Content.Client.CrateTimer.UI; // Убедись, что здесь тоже .UI

public sealed class TimerCrateBoundUserInterface : BoundUserInterface
{
    private TimerCrateWindow? _window;

    public TimerCrateBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey) { }

    protected override void Open()
    {
        base.Open();
        _window = new TimerCrateWindow();
        _window.OnClose += Close;

        _window.OnActivatePressed += () => SendMessage(new TimerCrateActivateMessage());

        _window.OpenCentered();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        if (state is not TimerCrateBoundUserInterfaceState cast) return;
        _window?.UpdateState(cast);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _window?.Dispose();
    }
}
