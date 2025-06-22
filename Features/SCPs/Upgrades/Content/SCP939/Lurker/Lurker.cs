using PlayerRoles;
using System;

namespace SwiftUHC.Features.SCPs.Upgrades.Content.SCP939.Lurker
{
    [UpgradePath(RoleTypeId.Scp939)]
    [Perk("939.Lurker", Rarity.Rare, PerkRestriction.SCP)]
    public class Lurker(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Lurker";

        public override string PathDescription => "Surprise your enemies.";

        public override Type[] AllUpgrades => [
            typeof(CeilingStalker),
            ];
    }
}
