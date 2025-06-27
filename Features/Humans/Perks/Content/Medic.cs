using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using PlayerRoles;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Medic", Rarity.Common)]
    public class Medic(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Medic";

        public override string Description => $"Shooting a teammate will heal them by {Amount} HP.";

        public virtual float Amount => 5f;

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

        private void OnHurting(PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker != Player || ev.Player.Role.GetFaction() != ev.Attacker.Role.GetFaction())
                return;

            ev.Player.Heal(Amount);
        }
    }
}
