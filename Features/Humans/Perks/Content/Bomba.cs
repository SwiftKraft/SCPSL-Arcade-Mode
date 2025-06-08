using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Utils;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Bomba", Rarity.Common)]
    public class Bomba(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Bomba";

        public override string Description => "You explode upon death.";

        public override void Init()
        {
            base.Init();
            PlayerEvents.Death += OnPlayerDeath;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Death -= OnPlayerDeath;
        }

        private void OnPlayerDeath(PlayerDeathEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            ExplosionUtils.ServerExplode(ev.Player.Position, new(ev.Player.ReferenceHub), ExplosionType.Grenade);
        }
    }
}
