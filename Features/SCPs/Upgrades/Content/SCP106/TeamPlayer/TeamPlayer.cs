using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.TeamPlayer
{
    public class TeamPlayer(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Team Player";

        public override string PathDescription => "Provides a lot of team bonuses.";

        public override Type[] AllUpgrades => [
            
            ];
    }
}
