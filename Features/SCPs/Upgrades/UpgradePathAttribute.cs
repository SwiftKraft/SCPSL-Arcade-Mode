using PlayerRoles;
using SwiftUHC.Utils.Interfaces;
using System;

namespace SwiftUHC.Features.SCPs.Upgrades
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UpgradePathAttribute(string id, RoleTypeId role) : Attribute, IWeight
    {
        public readonly string ID = id;
        public readonly RoleTypeId Role = role;

        public PerkAttribute Perk { get; set; }

        public int Weight => Perk == null ? 0 : Perk.Weight;
    }
}
