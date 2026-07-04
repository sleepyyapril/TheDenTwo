using Content.Shared._DEN.Loadout;
using Content.Shared.Clothing.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Content.Shared.Storage;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Station;

public abstract partial class SharedStationSpawningSystem
{
    private const string DefaultItemsSlot = "back";

    public void SpawnCharacterLoadout(HashSet<ProtoId<EntityLoadoutPrototype>> loadouts,
        EntityUid character,
        EntityCoordinates coordinates)
    {
        var sortedLoadouts = GetSortedPrototypes(loadouts);
        var items = GetItems(sortedLoadouts);
        var equipment = GetEquipment(sortedLoadouts);
        var inhand = GetInhand(sortedLoadouts, character);
        var storage = GetStorage(sortedLoadouts);

        SpawnCharacterEquipment(character, equipment, coordinates);
        SpawnCharacterInhand(character, inhand, coordinates);
        SpawnCharacterStorage(character, storage, coordinates);
        SpawnCharacterItems(character, items, coordinates);
    }

    private List<EntityLoadoutPrototype> GetSortedPrototypes(HashSet<ProtoId<EntityLoadoutPrototype>> loadouts)
    {
        var result = new List<EntityLoadoutPrototype>();

        foreach (var loadoutId in loadouts)
        {
            if (!PrototypeManager.TryIndex(loadoutId, out var loadout))
                continue;

            result.Add(loadout);
        }

        result.Sort((a, b) =>
            a.Priority.CompareTo(b.Priority));
        return  result;
    }

    private void SpawnCharacterEquipment(EntityUid character,
        Dictionary<string, EntProtoId> equipment,
        EntityCoordinates coordinates
    )
    {
        if (!InventorySystem.TryGetSlots(character, out var slotDefinitions))
            return;

        foreach (var slot in slotDefinitions)
        {
            if (!equipment.TryGetValue(slot.Name, out var equipmentId)
                || string.IsNullOrEmpty(equipmentId))
                continue;

            var equipmentEntity = Spawn(equipmentId, coordinates);
            InventorySystem.TryEquip(character, equipmentEntity, slot.Name, silent: true, force: true);
        }
    }

    private void SpawnCharacterInhand(EntityUid character,
        List<EntProtoId> inhand,
        EntityCoordinates coordinates
    )
    {
        if (!_handsQuery.TryComp(character, out var handsComponent))
            return;

        var coords = coordinates;

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var prototype in inhand)
        {
            var inhandEntity = Spawn(prototype, coords);

            if (_handsSystem.TryGetEmptyHand((character, handsComponent), out var emptyHand))
            {
                _handsSystem.TryPickup(character, inhandEntity, emptyHand, checkActionBlocker: false, handsComp: handsComponent);
            }
        }
    }

    private void SpawnCharacterStorage(EntityUid character,
        Dictionary<string, List<EntProtoId>> storage,
        EntityCoordinates coordinates
    )
    {
        if (storage.Count <= 0)
            return;

        _inventoryQuery.TryComp(character, out var inventoryComp);

        foreach (var (slotName, entProtos) in storage)
        {
            if (entProtos.Count == 0)
                continue;

            if (inventoryComp == null ||
                !InventorySystem.TryGetSlotEntity(character,
                    slotName,
                    out var slotEnt,
                    inventoryComponent: inventoryComp) ||
                !_storageQuery.TryComp(slotEnt, out var storageComponent))
                continue;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var entProto in entProtos)
            {
                var spawnedEntity = Spawn(entProto, coordinates);
                _storage.Insert(slotEnt.Value, spawnedEntity, out _, storageComp: storageComponent, playSound: false);
            }
        }
    }

    private void SpawnCharacterItems(EntityUid character,
        List<EntProtoId> items,
        EntityCoordinates coordinates
    )
    {
        if (!_inventoryQuery.TryComp(character, out var inventoryComp))
            return;

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var item in items)
        {
            var spawnedEntity = Spawn(item, coordinates);
            var (bestSlot, inside) = GetSlotSpawnable((character, inventoryComp), spawnedEntity);

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (bestSlot == null
                && TryComp<HandsComponent>(character, out var handsComponent)
                && _handsSystem.TryGetEmptyHand((character, handsComponent), out var emptyHand))
            {
                _handsSystem.TryPickup(character, spawnedEntity, emptyHand, checkActionBlocker: false, handsComp: handsComponent);
                continue;
            }

            if (bestSlot == null)
                continue;

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (inside
                && InventorySystem.TryGetSlotEntity(character,
                    bestSlot.Name,
                    out var slotEnt,
                    inventoryComponent: inventoryComp)
                && _storageQuery.TryComp(slotEnt, out var storageComponent)
                && _storage.CanInsert(slotEnt.Value, spawnedEntity, out _))
            {
                _storage.Insert(slotEnt.Value, spawnedEntity, out _, storageComp: storageComponent, playSound: false);
                continue;
            }

            if (!inside)
                InventorySystem.TryEquip(character, spawnedEntity, bestSlot.Name, silent: true, force: true);
        }
    }

    private (SlotDefinition?, bool inside) GetSlotSpawnable(Entity<InventoryComponent> character, EntityUid item)
    {
        var slotFlags = SlotFlags.All;

        if (TryComp<ClothingComponent>(item, out var clothingComponent))
        {
            slotFlags = clothingComponent.Slots;
        }

        var enumerator = InventorySystem.GetSlotEnumerator(character.Owner, slotFlags);

        while (enumerator.MoveNext(out var slot))
        {
            if (!InventorySystem.TryGetSlot(character, slot.ID, out var slotDefinition, character.Comp))
                continue;

            if ((slotDefinition.SlotFlags & slotFlags) != 0x0
                && slot.ContainedEntity == null
                && InventorySystem.CanEquip(character,
                    item,
                    slotDefinition.Name,
                    out _))
                return (slotDefinition, false);

            if (slot.ContainedEntity == null)
                continue;

            if (!InventorySystem.TryGetSlotContainer(character.Owner,
                    slot.ID,
                    out var containerSlot,
                    out _,
                    inventory: character.Comp)
                || containerSlot.ContainedEntity is not { Valid: true } slotEnt)
                continue;

            if (!TryComp<StorageComponent>(slotEnt, out var storageComponent))
                continue;

            if (_storage.CanInsert(slotEnt, item, out _, storageComponent))
                return (slotDefinition, true);
        }
;
        return (null, false);
    }

    private List<EntProtoId> GetItems(List<EntityLoadoutPrototype> sortedLoadouts)
    {
        var result = new List<EntProtoId>();

        foreach (var loadout in sortedLoadouts)
        {
            foreach (var item in loadout.Items)
            {
                result.Add(item);
            }
        }

        return result;
    }

    private Dictionary<string, EntProtoId> GetEquipment(List<EntityLoadoutPrototype> sortedLoadouts)
    {
        var result = new Dictionary<string, EntProtoId>();

        foreach (var loadout in sortedLoadouts)
        {
            foreach (var (key, value) in loadout.Equipment)
            {
                result[key] = value;
            }
        }

        return result;
    }

    private List<EntProtoId> GetInhand(List<EntityLoadoutPrototype> sortedLoadouts,
        EntityUid character)
    {
        var result = new List<EntProtoId>();
        var hands = _handsSystem.GetHandCount(character);
        var inhandItems = 0;

        foreach (var loadout in sortedLoadouts)
        {
            foreach (var inhand in loadout.Inhand)
            {
                inhandItems++;

                if (inhandItems > hands)
                    break;

                result.Add(inhand);
            }
        }

        return result;
    }

    private Dictionary<string, List<EntProtoId>> GetStorage(List<EntityLoadoutPrototype> sortedLoadouts)
    {
        var result = new Dictionary<string, List<EntProtoId>>();

        foreach (var loadout in sortedLoadouts)
        {
            foreach (var (key, value) in loadout.Storage)
            {
                if (!result.ContainsKey(key))
                    result[key] = new();

                result[key].AddRange(value);
            }
        }

        return result;
    }
}
