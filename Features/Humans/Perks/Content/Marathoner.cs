using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Marathoner", Rarity.Common)]
    public class Marathoner(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Marathoner";

        public override string Description => "Increases your max stamina by a large margin.";

        public virtual float Multiplier => 2f;

        float originalMax;

        public override void Init()
        {
            base.Init();
            originalMax = Player.GetStatModule<StaminaStat>().MaxValue;
            Player.GetStatModule<StaminaStat>().MaxValue *= Multiplier;
        }

        public override void Remove()
        {
            base.Remove();
            Player.GetStatModule<StaminaStat>().MaxValue = originalMax;
        }
    }
}
