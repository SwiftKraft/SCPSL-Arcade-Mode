using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftUHC.Features.Humans.Perks.Content.Gambler
{
    public class GamblerGravityNegative : GamblerEffectBase
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Doubled your gravity.";

        public override void Effect(Player player) => player.Gravity *= 2f;
    }
}
