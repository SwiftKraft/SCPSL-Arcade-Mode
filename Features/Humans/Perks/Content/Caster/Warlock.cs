using System;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    [Perk("Warlock", Rarity.Legendary)]
    public class Warlock(PerkInventory inv) : CasterBase(inv)
    {
        public override float RegularCooldown => 8f;

        public override string Name => "Warlock";

        public override Type[] ListSpells() => [
            typeof(CurseOfPain),
            typeof(BoltOfDarkness),
            typeof(RayOfDarkness)
            ];
    }
}
