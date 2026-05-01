using Content.Shared._Exodus.Crucible;
using Robust.Client.GameObjects;

namespace Content.Client._Exodus.Crucible.UI;

public sealed class CrucibleBoundUserInterface : BoundUserInterface
{
    private CrucibleWindow? _window;

    public CrucibleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey) { }

    protected override void Open()
    {
        base.Open();
        _window = new CrucibleWindow();
        _window.OnClose += Close;
        _window.OnAction += button => SendMessage(new CrucibleUiSendMessage(button));
        _window.OpenCentered();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);
        if (state is CrucibleBoundUserInterfaceState crucibleState)
            _window?.UpdateState(crucibleState);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _window?.Dispose();
    }
}
