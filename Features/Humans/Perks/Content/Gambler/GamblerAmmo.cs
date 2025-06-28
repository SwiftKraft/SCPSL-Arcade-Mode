using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    public class GamblerAmmo : GamblerEffectBase
    {
        public override bool Positive => true;

        public override int Weight => 1;

        public override string Explanation => "Ammo OVERFLOW...";

        public override void Effect(Player player)
        {
            player.AddAmmo(ItemType.Ammo12gauge, 200);
            player.AddAmmo(ItemType.Ammo44cal, 200);
            player.AddAmmo(ItemType.Ammo556x45, 400);
            player.AddAmmo(ItemType.Ammo762x39, 400);
            player.AddAmmo(ItemType.Ammo9x19, 600);
        }
    }
}
