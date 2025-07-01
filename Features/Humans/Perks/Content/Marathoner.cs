using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Marathoner", Rarity.Common)]
    public class Marathoner(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public override string Name => "Marathoner";

        public override string Description => $"Regenerates your stamina by {Amount} every {Cooldown} seconds.";

        public override string PerkDescription => "";

        public virtual float Amount => 5f;

        public override string ReadyMessage => "";

        public override float Cooldown => 8f;

        public override void Effect()
        {
            if (Player.StaminaRemaining < Player.GetStatModule<StaminaStat>().MaxValue)
                Player.StaminaRemaining += Amount;
        }
    }
}
