using System.Linq;
using Content.Client.Stylesheets;
using Content.Shared._DEN.Loadout;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client._DEN.Lobby.UI.Loadouts;

public sealed partial class LoadoutProfilesContainer
{
    private readonly Dictionary<ProtoId<LoadoutCategoryPrototype>, LoadoutCategoryPrototype> _loadoutCategories = new();

    private readonly Dictionary<ProtoId<LoadoutCategoryPrototype>, ProtoId<LoadoutCategoryPrototype>>
        _parents = new();
    private readonly Dictionary<ProtoId<EntityLoadoutPrototype>, EntityLoadoutPrototype> _loadoutEntities = new();
    private readonly HashSet<ProtoId<LoadoutCategoryPrototype>> _root = new();

    public ProtoId<LoadoutCategoryPrototype>? CurrentCategory = null;

    private void OnClickEditLoadouts(DenLoadoutProfile loadoutProfile)
    {
        LoadoutItemSelection.Visible = !LoadoutContainer.Visible;
        LoadoutProfileSelection.Visible = !LoadoutContainer.Visible;
    }

    private void CacheLoadouts()
    {
        _loadoutCategories.Clear();
        _parents.Clear();
        _loadoutEntities.Clear();

        foreach (var category in _protoMan.EnumeratePrototypes<LoadoutCategoryPrototype>())
        {
            foreach (var child in category.SubCategories)
            {
                _parents[child] = category;
            }

            if (category.Root && !_parents.ContainsKey(category))
                _root.Add(category);

            _loadoutCategories.Add(category, category);
        }

        foreach (var loadoutItem in _protoMan.EnumeratePrototypes<EntityLoadoutPrototype>())
        {
            _loadoutEntities.Add(loadoutItem, loadoutItem);
        }
    }

    public void RefreshLoadoutItemCategories()
    {
        HashSet<LoadoutCategoryPrototype> allCategories;

        if (CurrentCategory != null)
        {
            allCategories = _loadoutCategories[CurrentCategory.Value]
                .SubCategories
                .Select(c => _loadoutCategories[c])
                .ToHashSet();
        }
        else
        {
            allCategories = _root
                .Select(c => _loadoutCategories[c])
                .ToHashSet();
        }

        var (itemCategories, subCategories) = GetCategoryTypes(allCategories);

        foreach (var itemCategory in itemCategories)
        {
            var button = new Button
            {
                Margin = new Thickness(5, 2),
                HorizontalExpand = true,
                SetHeight = 35
            };

            LoadoutItemSelection.LoadoutItemCategoryContainer.AddChild(button);
        }

        foreach (var itemCategory in subCategories)
        {
            var button = new Button
            {
                Margin = new Thickness(5, 2),
                HorizontalExpand = true,
                SetHeight = 35,
                StyleClasses = { "ButtonColorGreen" }
            };

            LoadoutItemSelection.LoadoutItemCategoryContainer.AddChild(button);
        }
    }

    private (HashSet<LoadoutCategoryPrototype> ItemCategories,
        HashSet<LoadoutCategoryPrototype> SubCategories) GetCategoryTypes(HashSet<LoadoutCategoryPrototype> allCategories)
    {
        var itemCategories = new HashSet<LoadoutCategoryPrototype>();
        var subCategories = new HashSet<LoadoutCategoryPrototype>();

        foreach (var category in allCategories)
        {
            if (category.SubCategories.Count > 0)
            {
                subCategories.Add(category);
                continue;
            }

            itemCategories.Add(category);
        }

        return (itemCategories, subCategories);
    }
}
