using LabApi.Features.Wrappers;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("BombHen", Rarity.Legendary)]
    public class BombHen(PerkInventory inv) : PerkCooldownBase(inv)
    {
        public override string Name => "Bomb Hen";

        public override string Description => "Lay an explosive egg (it can also kill you). " + base.Description;

        public override float Cooldown => 35f;

        public override void Effect() => TimedGrenadeProjectile.SpawnActive(Player.Position, ItemType.GrenadeHE, Player);

        public override void Tick()
        {
            base.Tick();
            Trigger();
        }
    }
}
