using System;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    [Perk("Wizard", Rarity.Legendary)]
    public class Wizard(PerkInventory inv) : CasterBase(inv)
    {
        public override string Name => "Wizard";

        public override float RegularCooldown => 10f;

        public override Type[] ListSpells() => [
            typeof(Fireball),
            typeof(IceBolt),
            typeof(MagicMissile),
            typeof(LightArrow)
            ];
    }
}
