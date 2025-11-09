using System.Linq;
using Content.Server._DEN.CartridgeLoader.Cartridges;
using Content.Server.NameIdentifier;
using Content.Shared._DEN.Access.Systems;
using Content.Shared._DEN.CartridgeLoader.Cartridges;
using Content.Shared._DEN.NanoChat;
using Content.Shared.Access.Components;
using Content.Shared.NameIdentifier;
using Content.Shared.PDA;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._DEN.NanoChat;

public sealed partial class NanoChatCardSystem : SharedNanoChatCardSystem
{
    [Dependency] private readonly NameIdentifierSystem _nameIdentifier = null!;
    [Dependency] private readonly NanoChatCartridgeSystem _nanoChatCartridge = null!;

    private const string DefaultFullName = "Unknown";
    private const string DefaultJobTitle = "Unknown";

    private readonly ProtoId<NameIdentifierGroupPrototype> _nanoChatIds = "NanoChatIds";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NanoChatCardComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<NanoChatCardComponent, EntGotInsertedIntoContainerMessage>(OnCardInserted);
        SubscribeLocalEvent<NanoChatCardComponent, EntGotRemovedFromContainerMessage>(OnCardRemoved);
        SubscribeLocalEvent<NanoChatCardComponent, IdCardNameUpdatedEvent>(OnNameUpdated);
        SubscribeLocalEvent<NanoChatCardComponent, IdCardJobTitleUpdatedEvent>(OnJobTitleUpdated);

        // Synchronizing messages, conversations, users
        InitializeSync();
    }

    private void OnMapInit(Entity<NanoChatCardComponent> ent, ref MapInitEvent args)
    {
        if (ent.Comp.PersonalNumber != null)
            return;

        _nameIdentifier.GenerateUniqueName(ent, _nanoChatIds, out var number);
        var nanoChatId = (uint) number;
        SetNumber(ent.AsNullable(), nanoChatId);
        CacheUser(ent, nanoChatId);
    }

    private void OnCardInserted(Entity<NanoChatCardComponent> ent, ref EntGotInsertedIntoContainerMessage msg)
    {
        if (msg.Container.ID != PdaComponent.PdaIdSlotId)
            return;

        ent.Comp.LoaderUid = msg.Container.Owner;
        Dirty(ent);
    }

    private void OnCardRemoved(Entity<NanoChatCardComponent> ent, ref EntGotRemovedFromContainerMessage msg)
    {
        if (msg.Container.ID != PdaComponent.PdaIdSlotId)
            return;

        ent.Comp.LoaderUid = null;
        Dirty(ent);
    }

    private void OnNameUpdated(Entity<NanoChatCardComponent> ent, ref IdCardNameUpdatedEvent args)
    {
        if (ent.Comp.PersonalNumber == null
            || !_users.TryGetValue(ent.Comp.PersonalNumber.Value, out _))
            return;

        CacheUser(ent, ent.Comp.PersonalNumber.Value);
    }

    private void OnJobTitleUpdated(Entity<NanoChatCardComponent> ent, ref IdCardJobTitleUpdatedEvent args)
    {
        if (ent.Comp.PersonalNumber == null
            || !_users.TryGetValue(ent.Comp.PersonalNumber.Value, out _))
            return;

        CacheUser(ent, ent.Comp.PersonalNumber.Value);
    }

    private void CacheUser(Entity<NanoChatCardComponent> ent, uint nanoChatId)
    {
        if (!TryComp<IdCardComponent>(ent, out var idCard))
            return;

        var name = idCard.FullName ?? DefaultFullName;
        var jobTitle = idCard.LocalizedJobTitle ?? DefaultJobTitle;

        var newUser = new NanoChatUser(nanoChatId, name, jobTitle);
        _users[newUser.UniqueId] = newUser;
        _updatedUsers.Add(nanoChatId);

        var usersClone = _users.ShallowClone();
        var ev = new NanoChatUsersUpdatedEvent(usersClone);
        RaiseLocalEvent(ref ev);
    }
}
