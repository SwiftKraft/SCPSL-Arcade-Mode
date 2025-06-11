namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("SlotUpgrade", Rarity.Legendary)]
    public class PerkSlotUpgrade(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Perk Slot Upgrade";

        public override string Description => $"Gives you +{Amount} perk slots (excluding the one it takes up).";

        public virtual int Amount => 3;

        public override void Init()
        {
            base.Init();
            Inventory.Limit += Amount + 1;
        }

        public override void Remove()
        {
            base.Remove();
            Inventory.Limit -= Amount + 1;
        }
    }
}
