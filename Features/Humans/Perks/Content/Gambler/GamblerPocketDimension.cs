using CustomPlayerEffects;
using LabApi.Features.Wrappers;

namespace SwiftUHC.Features.Humans.Perks.Content.Gambler
{
    public class GamblerPocketDimension : GamblerEffectBase
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Let's gamble again.";

        public override void Effect(Player player) => player.EnableEffect<PocketCorroding>();
    }
}
