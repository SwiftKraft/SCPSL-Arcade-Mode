using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class Druid(PerkInventory inv) : CasterBase(inv)
    {
        public override float RegularCooldown => 8f;

        public override string Name => "Druid";

        public override Type[] ListSpells() => [
            typeof(ThornShot) 
            ];
    }
}
