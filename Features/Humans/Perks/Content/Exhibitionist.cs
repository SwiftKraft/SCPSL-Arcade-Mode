using LabApi.Features.Wrappers;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Exhib", Rarity.Secret)]
    public class Exhibitionist(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public override string Name => "Exhibitionist";

        public override string Description => "Flash enemies around you. " + base.Description;

        public override float Cooldown => 20f;

        public override void Effect() => TimedGrenadeProjectile.SpawnActive(Player.Position, ItemType.GrenadeFlash, Player, 0.1f);
    }
}
