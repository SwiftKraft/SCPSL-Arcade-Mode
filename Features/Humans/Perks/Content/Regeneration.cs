using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Regeneration", Rarity.Common)]
    public class Regeneration(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Regeneration";
        public override string Description => $"When you have >{HealthThreshold} HP, heal {Rate} HP/s when not taking damage.";

        public virtual float HealthThreshold => 60f;
        public virtual float Rate => 3f;

        public override void Tick()
        {
            base.Tick();

            if (Player.Health >= HealthThreshold && Player.Health < Player.MaxHealth)
                Player.Heal(Rate * Time.fixedDeltaTime);
        }
    }
}
