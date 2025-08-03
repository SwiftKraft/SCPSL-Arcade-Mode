using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features.Humans.Perks;

namespace SwiftArcadeMode.Features.Scoring.Events
{
    public class PerkScore : ScoreEventBase
    {
        public override void Disable() => PerkSpawner.OnPickedUpPerk -= OnPickedUpPerk;

        public override void Enable() => PerkSpawner.OnPickedUpPerk += OnPickedUpPerk;

        protected virtual void OnPickedUpPerk(Player p, PerkAttribute obj) => p.AddScore(obj.Profile.Rarity switch
        {
            Rarity.Secret => 50,
            Rarity.Mythic => 35,
            Rarity.Legendary => 20,
            Rarity.Epic => 10,
            Rarity.Rare => 6,
            Rarity.Uncommon => 4,
            Rarity.Common => 2,
            _ => 1,
        });

        public override void Tick() { }
    }
}
