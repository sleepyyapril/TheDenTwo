using System.Linq;
using Content.Server.Database;
using Content.Shared._DEN.Loadout;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server.Preferences.Managers;

public sealed partial class ServerPreferencesManager
{
    private ProtoId<EntityLoadoutPrototype> AsLoadoutPrototype(string protoId)
    {
        return protoId;
    }

    public ProtoId<JobPrototype> AsJobPrototype(string protoId)
    {
        return protoId;
    }
}
