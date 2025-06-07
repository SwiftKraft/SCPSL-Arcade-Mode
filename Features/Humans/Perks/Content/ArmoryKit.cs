using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("ArmoryKit", Rarity.Uncommon)]
    public class ArmoryKit(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public override string Name => "Armory Kit";

        public override string Description => $"Receive a random throwable, SCP items excluded. " + base.Cooldown;

        public override float Cooldown => 60f;

        public override ItemType ItemType => Random.Range(0, 2) switch
        {
            0 => ItemType.GrenadeFlash,
            1 => ItemType.GrenadeHE,
            _ => ItemType.GrenadeFlash
        };
    }
}
