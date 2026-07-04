using Content.Shared._DEN.Requirements.PlayerRequirements;
using Content.Shared._DEN.Traits.TraitFunctions;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared._DEN.Loadout;

[Prototype]
public sealed partial class EntityLoadoutPrototype : IInheritingPrototype, IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <inheritdoc/>
    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<EntityLoadoutPrototype>))]
    public string[]? Parents { get; private set; }

    /// <inheritdoc/>
    [AbstractDataField]
    public bool Abstract { get; private set; }

    /// <summary>
    /// The name of this loadout.
    /// </summary>
    [DataField]
    public LocId Name { get; private set; } = string.Empty;

    /// <summary>
    /// The description of this loadout.
    /// </summary>
    [DataField]
    public LocId? Description { get; private set; }

    [DataField]
    public int Priority;

    /// <summary>
    /// Don't apply this loadout to entities this whitelist IS NOT valid for.
    /// </summary>
    [DataField]
    public EntityWhitelist? Whitelist;

    /// <summary>
    /// Don't apply this loadout to entities this whitelist IS valid for. (hence, a blacklist)
    /// </summary>
    [DataField]
    public EntityWhitelist? Blacklist;

    /// <summary>
    /// Adds a trait to a category, allowing you to limit the selection of some traits to the settings of that category.
    /// </summary>
    [DataField]
    public ProtoId<LoadoutCategoryPrototype>? Category;

    /// <summary>
    /// A list of functions associated with this loadout item.
    /// </summary>
    [DataField("functions")]
    public List<ITraitFunction> TraitFunctions = new();

    /// <summary>
    /// Whether or not this loadout can be selected in the character editor.
    /// </summary>
    [DataField("characterEditorSelectable")]
    public bool Selectable = true;

    /// <summary>
    ///     A list of requirements to use this trait.
    /// </summary>
    [DataField]
    public List<IPlayerRequirement> Requirements = new();

    /// <summary>
    /// The full list of items to spawn in this loadout.
    /// </summary>
    [DataField]
    public List<EntProtoId> Items = new();

    /// <summary>
    /// Any items that, by default, spawn in specific equipment slots.
    /// </summary>
    /// <remarks>
    /// Players can override this.
    /// </remarks>
    [DataField]
    public Dictionary<string, EntProtoId> Equipment { get; set; } = new();

    /// <summary>
    /// Any items that, by default, spawn in available inhand slots.
    /// </summary>
    /// <remarks>
    /// Players can override this.
    /// </remarks>
    [DataField]
    public List<EntProtoId> Inhand { get; set; } = new();

    /// <summary>
    /// Any items that, by default, spawn in specific storages.
    /// </summary>
    /// <remarks>
    /// Players can override this.
    /// </remarks>
    [DataField]
    public Dictionary<string, List<EntProtoId>> Storage { get; set; } = new();
}

