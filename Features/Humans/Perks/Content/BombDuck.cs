using LabApi.Events.Handlers;
using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("BombDuck", Rarity.Legendary)]
    public class BombDuck(PerkInventory inv) : BombHen(inv)
    {
        public override string Name => "Bomb Duck";

        public override string PerkDescription => "Lay an explosive <i>duck</i> egg (your grenades don't damage you). ";

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurting += OnHurting;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurting -= OnHurting;
        }

        private void OnHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Player != Player || ev.DamageHandler is not ExplosionDamageHandler || ev.Attacker != Player)
                return;

            ev.IsAllowed = false;
        }
    }
}
