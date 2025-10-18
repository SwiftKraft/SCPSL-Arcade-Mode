using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class Warlock(PerkInventory inv) : CasterBase(inv)
    {
        public override float RegularCooldown => 6f;

        public override string Name => "Warlock";

        public override Type[] ListSpells() => [
            
            ];
    }
}
