using PlayerRoles;
using System;

namespace SwiftUHC.Features.SCPs.Upgrades
{
    public class UpgradePathAttribute(RoleTypeId role) : Attribute
    {
        public readonly RoleTypeId Role = role;

        public PerkAttribute Perk { get; set; }
    }
}
