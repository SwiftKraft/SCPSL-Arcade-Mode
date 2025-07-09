using InventorySystem.Items.Firearms.Modules;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("MicroMissiles", Rarity.Secret)]
    public class MicroMissiles(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Micro Missiles";

        public override string Description => "Every shot creates an explosive projectile.";

        public override void Init()
        {
            base.Init();
            PlayerEvents.ShootingWeapon += OnShootingWeapon;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.ShootingWeapon -= OnShootingWeapon;
        }

        private void OnShootingWeapon(LabApi.Events.Arguments.PlayerEvents.PlayerShootingWeaponEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            ExplosiveGrenadeProjectile pick = TimedGrenadeProjectile.SpawnActive(Player.Camera.position, ItemType.GrenadeHE, Player, 3d) as ExplosiveGrenadeProjectile;
            pick.Base.MaxRadius = 2f;

            Rocketeer.ConvertRocket(Player, pick, 45f);
        }
    }
}
