using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Streamer", Rarity.Secret)]
    public class Streamer(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Streamer";

        public override string Description => $"Every spectator currently watching you will heal you {RatePerPlayer} HP/s.";

        public virtual float RatePerPlayer => 3f;

        public override void Tick()
        {
            base.Tick();

            if (Player.CurrentSpectators.Count > 0)
                Player.Heal(RatePerPlayer * Time.fixedDeltaTime * Player.CurrentSpectators.Count);
        }
    }
}
