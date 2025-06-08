using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using MEC;
using UnityEngine;
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
            PlayerEvents.Dying += OnPlayerDying;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Dying -= OnPlayerDying;
        }

        private void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            Vector3 position = Player.Position;
            Timing.CallDelayed(0.1f, () => { ExplosionUtils.ServerExplode(position, new(ev.Player.ReferenceHub), ExplosionType.Grenade); });
        }
    }
}
