using Content.Shared._DEN.Loadout;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Station;

public abstract partial class SharedStationSpawningSystem
{
    private const string DefaultItemsSlot = "BACK";

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

    // TODO: go through every possible slot for an item
    private void SpawnCharacterItems(EntityUid character,
        List<EntProtoId> items,
        EntityCoordinates coordinates
    )
    {
        _inventoryQuery.TryComp(character, out var inventoryComp);

        if (inventoryComp == null)
            return;

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var item in items)
        {
            var spawnedEntity = Spawn(item, coordinates);

            if (!InventorySystem.TryGetSlotEntity(character, DefaultItemsSlot, out var storageUid, inventoryComp)
                || !_storageQuery.TryComp(storageUid, out var storageComponent))
                continue;

            _storage.Insert(storageUid.Value,
                spawnedEntity,
                out _,
                storageComp: storageComponent,
                playSound: false);
        }
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

        foreach (var loadout in sortedLoadouts)
        {
            foreach (var inhand in loadout.Inhand)
            {
                result.Add(inhand);
            }
        }

        var hands = _handsSystem.GetHandCount(character);
        var length = result.Count - hands;

        result.RemoveRange(hands, length);

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
