using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;
using Utils;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Bomba", Rarity.Common)]
    public class Bomba(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Bomba";

        public override string Description => "You drop a short fused grenade upon death.";

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

            TimedGrenadeProjectile.SpawnActive(Player.Position, ItemType.GrenadeHE, Player, 0.5f);
        }
    }
}
