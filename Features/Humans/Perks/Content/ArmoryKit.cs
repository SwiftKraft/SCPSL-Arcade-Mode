using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("ArmoryKit", Rarity.Uncommon)]
    public class ArmoryKit(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public override string Name => "Armory Kit";

        public override string Description => $"Receive a grenade. " + base.Description;

        public override float Cooldown => 60f;

        public override ItemType ItemType => ItemType.GrenadeHE;
    }
}
