using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftUHC.Features.SCPs.Upgrades.Content.SCP106.EndlessDecay
{
    public class EndlessDecay(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Endless Decay";

        public override string PathDescription => "Decay various things.";

        public override Type[] AllUpgrades => [
            
            ];
    }
}
