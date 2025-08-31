using SwiftArcadeMode.Features.Humans.Perks.Content;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP3114
{
    public class SlotUpgrade(UpgradePathPerkBase parent) : UpgradeBase<RandomPerks>(parent)
    {
        public override string Name => $"Slot Upgrade";

        public override string Description => "Get a slot upgrade.";

        public override void Init()
        {
            base.Init();
            if (!Player.HasPerk(typeof(PerkSlotUpgrade)))
                Player.GivePerk(typeof(PerkSlotUpgrade));
        }
    }
}
