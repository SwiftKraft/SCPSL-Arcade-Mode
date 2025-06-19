﻿using LabApi.Events.Handlers;
using PlayerStatsSystem;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Reaper", Rarity.Uncommon)]
    public class Reaper(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Reaper";

        public override string Description => $"Heal {Percentage * 100}% of damage dealt to enemies.";

        public virtual float Percentage => 0.05f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurt += OnPlayerHurt;
        }

        private void OnPlayerHurt(LabApi.Events.Arguments.PlayerEvents.PlayerHurtEventArgs ev)
        {
            if (ev.Attacker != Player || ev.DamageHandler is not StandardDamageHandler dmg)
                return;

            Player.Heal(dmg.Damage * Percentage);
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurt -= OnPlayerHurt;
        }
    }
}
