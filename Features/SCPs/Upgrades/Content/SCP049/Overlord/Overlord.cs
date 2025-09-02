using PlayerRoles;
using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.Overlord
{
    [UpgradePath(RoleTypeId.Scp049)]
    [Perk("049.Overlord", Rarity.Epic, PerkRestriction.SCP)]
    public class Overlord(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Overlord";

        public override string PathDescription => "Abuse your zombies!";

        public override Type[] AllUpgrades => [
            typeof(Siphon),
            typeof(Soulbound)
            ];
    }
}
