using Content.Shared._DEN.Loadout;
using Robust.Shared.Prototypes;

namespace Content.Client._DEN.Lobby.UI.Loadouts;

public sealed partial class DenLoadoutTab
{
    private void OnTryCreateCategory(DenLoadoutCategory? existingCategory = null)
    {
        if (_createCategoryPopup != null)
            return;

        _createCategoryPopup = new CreateCategoryPopup(existingCategory);
        _createCategoryPopup.OnSubmit += OnSubmitNewCategory;
        _createCategoryPopup.OnClose += OnCloseCategoryPopup;
        _createCategoryPopup.OpenCentered();
    }

    private void OnSubmitNewCategory(CategoryCreationRequest request)
    {
        if (string.IsNullOrEmpty(request.CategoryName))
            return;

        var guid = request.ExistingCategoryId ?? Guid.NewGuid();
        var priority = 100;
        var members = new HashSet<Guid>();

        if (_profile?.LoadoutCategories.TryGetValue(guid, out var loadout) == true)
        {
            priority = loadout.Priority;
            members.UnionWith(loadout.Members);
        }

        var category = new DenLoadoutCategory
        {
            Id = guid,
            Color = request.CategoryColor.ToHex(),
            Name = request.CategoryName,
            Priority = priority,
            Members = members
        };

        _profile = _profile?.WithLoadoutCategory(guid, category);
        OnPreferenceUpdated?.Invoke(_profile);

        RefreshCategories();
        OnCloseCategoryPopup();
    }

    private void OnTryCreateLoadout(DenLoadout? loadout = null)
    {
        if (_createLoadoutPopup != null)
            return;

        _createLoadoutPopup = new CreateLoadoutPopup(_popupCategories, loadout);
        _createLoadoutPopup.OnSubmit += OnSubmitNewLoadout;
        _createLoadoutPopup.OnClose += OnCloseLoadoutPopup;
        _createLoadoutPopup.OpenCentered();
    }

    private void OnSubmitNewLoadout(LoadoutCreationRequest request)
    {
        if (string.IsNullOrEmpty(request.LoadoutName))
            return;

        var guid = Guid.NewGuid();
        var priority = 100;
        var loadouts = new HashSet<ProtoId<EntityLoadoutPrototype>>();

        if (request.ExistingLoadoutId is { } loadoutProfileId
            && _profile?.LoadoutProfiles.TryGetValue(loadoutProfileId, out var loadoutProfile) == true)
        {
            guid = loadoutProfile.Id;
            priority = loadoutProfile.Priority;
            loadouts.UnionWith(loadoutProfile.Loadouts);
        }

        var loadout = new DenLoadout
        {
            Id = guid,
            Name = request.LoadoutName,
            LoadoutCategory = request.CategoryId,
            Priority = priority,
            Loadouts = loadouts,
        };

        _profile = _profile?.WithLoadoutProfile(guid, request.CategoryId, loadout);
        SetDirty(_profile);

        RefreshCategories();
        OnCloseLoadoutPopup();
    }

    private void OnCloseCategoryPopup()
    {
        _createCategoryPopup?.Orphan();
        _createCategoryPopup = null;
    }

    private void OnCloseLoadoutPopup()
    {
        _createLoadoutPopup?.Orphan();
        _createLoadoutPopup = null;
    }
}
