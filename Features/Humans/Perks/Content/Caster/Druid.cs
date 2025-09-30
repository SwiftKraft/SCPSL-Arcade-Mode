﻿using System;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    [Perk("Druid", Rarity.Legendary)]
    public class Druid(PerkInventory inv) : CasterBase(inv)
    {
        public override float RegularCooldown => 8f;

        public override string Name => "Druid";

        public override Type[] ListSpells() => [
            typeof(ThornShot),
            typeof(OrbOfNature),
            typeof(ThornVolley)
            ];
    }
}
