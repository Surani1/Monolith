using Content.Shared._Exodus.Crucible;
using Content.Shared.Containers.ItemSlots;
using Content.Server.Power.Components;
using Robust.Shared.Timing;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;

namespace Content.Server._Exodus.Crucible;

public sealed class CrucibleSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CrucibleComponent, CrucibleUiSendMessage>(OnUiMessage);
        SubscribeLocalEvent<CrucibleComponent, EntInsertedIntoContainerMessage>(OnItemChanged);
        SubscribeLocalEvent<CrucibleComponent, EntRemovedFromContainerMessage>(OnItemChanged);
    }

    private void OnItemChanged<T>(EntityUid uid, CrucibleComponent component, ref T args)
    {
        UpdateUI(uid, component);
    }

    private void OnUiMessage(EntityUid uid, CrucibleComponent component, CrucibleUiSendMessage msg)
    {
        if (TryComp<ApcPowerReceiverComponent>(uid, out var power) && !power.Powered)
            return;

        if (msg.Button == CrucibleUiButton.Start) StartProcess(uid, component);
        else if (msg.Button == CrucibleUiButton.Abort) AbortProcess(uid, component);
    }

    private void StartProcess(EntityUid uid, CrucibleComponent component)
    {
        if (component.IsProcessing) return;
        if (!_itemSlots.TryGetSlot(uid, CrucibleComponent.SlotId, out var slot) || slot.Item == null) return;

        if (TryComp<CrucibleRecipeComponent>(slot.Item.Value, out var recipe))
        {
            component.IsProcessing = true;
            component.TotalTime = recipe.ProcessingTime;
            component.FinishTime = _timing.CurTime + recipe.ProcessingTime;
            _itemSlots.SetLock(uid, CrucibleComponent.SlotId, true);
            _appearance.SetData(uid, CrucibleVisuals.Active, true);
            UpdateUI(uid, component);
        }
    }

    private void AbortProcess(EntityUid uid, CrucibleComponent component)
    {
        if (!component.IsProcessing) return;
        component.IsProcessing = false;
        _itemSlots.SetLock(uid, CrucibleComponent.SlotId, false);
        _appearance.SetData(uid, CrucibleVisuals.Active, false);
        UpdateUI(uid, component);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<CrucibleComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (!comp.IsProcessing) continue;

            if (TryComp<ApcPowerReceiverComponent>(uid, out var power) && !power.Powered)
            {
                AbortProcess(uid, comp);
                continue;
            }

            if (_timing.CurTime < comp.FinishTime) continue;
            FinishProcess(uid, comp);
        }
    }

    private void FinishProcess(EntityUid uid, CrucibleComponent component)
    {
        component.IsProcessing = false;
        _appearance.SetData(uid, CrucibleVisuals.Active, false);
        _itemSlots.SetLock(uid, CrucibleComponent.SlotId, false);

        if (_itemSlots.TryGetSlot(uid, CrucibleComponent.SlotId, out var slot) && slot.Item != null)
        {
            if (TryComp<CrucibleRecipeComponent>(slot.Item.Value, out var recipe))
            {
                var coords = Transform(uid).Coordinates;
                EntityManager.DeleteEntity(slot.Item.Value);
                EntityManager.SpawnEntity(recipe.ResultEntity, coords);
            }
        }
        UpdateUI(uid, component);
    }

    private void UpdateUI(EntityUid uid, CrucibleComponent component)
    {
        string? itemName = null;
        if (_itemSlots.TryGetSlot(uid, CrucibleComponent.SlotId, out var slot) && slot.Item != null)
            itemName = Name(slot.Item.Value);

        var powered = !TryComp<ApcPowerReceiverComponent>(uid, out var power) || power.Powered;
        _ui.SetUiState(uid, CrucibleUiKey.Key, new CrucibleBoundUserInterfaceState(
            itemName, component.IsProcessing, component.FinishTime, component.TotalTime, powered));
    }
}
