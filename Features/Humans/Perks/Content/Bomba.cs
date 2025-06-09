using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Bomba", Rarity.Common)]
    public class Bomba(PerkInventory inv) : PerkKillBase(inv)
    {
        public override string Name => "Bomba";

        public override string Description => "You drop a short fused grenade upon death.";

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            TimedGrenadeProjectile.SpawnActive(Player.Position, ItemType.GrenadeHE, Player, 0.5f);
        }
    }
}
