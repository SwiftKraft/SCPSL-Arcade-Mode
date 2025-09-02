using InventorySystem.Items.Firearms.Modules;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Resupplier", Rarity.Uncommon)]
    public class Resupplier(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Resupplier";

        public override string Description => "Reloading no longer requires ammo.";

        public override void Init()
        {
            base.Init();

            Player.SetAmmo(ItemType.Ammo12gauge, 1);
            Player.SetAmmo(ItemType.Ammo9x19, 1);
            Player.SetAmmo(ItemType.Ammo44cal, 1);
            Player.SetAmmo(ItemType.Ammo556x45, 1);
            Player.SetAmmo(ItemType.Ammo762x39, 1);

            PlayerEvents.ReloadingWeapon += OnReloadingWeapon;
            PlayerEvents.AimedWeapon += OnAimedWeapon;
            PlayerEvents.DroppingAmmo += OnDroppingAmmo;
        }

        public override void Remove()
        {
            base.Remove();

            PlayerEvents.ReloadingWeapon -= OnReloadingWeapon;
            PlayerEvents.AimedWeapon -= OnAimedWeapon;
            PlayerEvents.DroppingAmmo -= OnDroppingAmmo;
        }

        private void OnAimedWeapon(PlayerAimedWeaponEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            Player.SetAmmo(ItemType.Ammo12gauge, 1);
            Player.SetAmmo(ItemType.Ammo9x19, 1);
            Player.SetAmmo(ItemType.Ammo44cal, 1);
            Player.SetAmmo(ItemType.Ammo556x45, 1);
            Player.SetAmmo(ItemType.Ammo762x39, 1);
        }

        private void OnDroppingAmmo(PlayerDroppingAmmoEventArgs ev)
        {
            if (ev.Player == Player)
                ev.IsAllowed = false;
        }

        protected void OnReloadingWeapon(PlayerReloadingWeaponEventArgs ev)
        {
            if (ev.Player != Player || Player.CurrentItem == null || Player.CurrentItem.Type == ItemType.GunSCP127 || Player.CurrentItem.Type == ItemType.ParticleDisruptor || Player.CurrentItem is not FirearmItem item || !item.Base.TryGetModule(out IPrimaryAmmoContainerModule mod) || !item.Base.TryGetModule(out IReloaderModule mod2) || mod2.IsReloadingOrUnloading || mod.AmmoType == ItemType.None)
                return;

            Player.SetAmmo(mod.AmmoType, (ushort)(mod.AmmoMax - mod.AmmoStored + 1));
        }
    }
}
