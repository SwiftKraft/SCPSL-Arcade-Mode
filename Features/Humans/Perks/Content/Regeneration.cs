using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Regeneration", Rarity.Common)]
    public class Regeneration(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Regeneration";
        public override string Description => $"When you have >{HealthThresholdPercentage * 100f}% HP, heal {Rate} HP/s.";

        public virtual float HealthThresholdPercentage => 0.6f;
        public virtual float Rate => 3f;

        public override void Tick()
        {
            base.Tick();

            if (Player.Health / Player.MaxHealth >= HealthThresholdPercentage && Player.Health < Player.MaxHealth)
                Player.Heal(Rate * Time.fixedDeltaTime);
        }
    }
}
