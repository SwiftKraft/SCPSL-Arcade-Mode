﻿using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    public class GamblerGravityNegative : GamblerEffectBase
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Doubled your gravity.";

        public override void Effect(Player player) => player.Gravity *= 2f;
    }
}
