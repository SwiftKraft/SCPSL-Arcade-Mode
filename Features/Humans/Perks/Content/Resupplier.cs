using InventorySystem.Items.Firearms.Modules;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Resupplier", Rarity.Uncommon)]
    public class Resupplier(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Resupplier";

        public override string Description => "Reloading no longer requires ammo.";

        public override void Init()
        {
            base.Init();

            Player.AddAmmo(ItemType.Ammo12gauge, 1);
            Player.AddAmmo(ItemType.Ammo9x19, 1);
            Player.AddAmmo(ItemType.Ammo44cal, 1);
            Player.AddAmmo(ItemType.Ammo556x45, 1);
            Player.AddAmmo(ItemType.Ammo762x39, 1);

            PlayerEvents.ReloadingWeapon += OnReloadingWeapon;
            PlayerEvents.DroppedAmmo += OnDroppedAmmo;
        }

        public override void Remove()
        {
            base.Remove();

            PlayerEvents.ReloadingWeapon -= OnReloadingWeapon;
            PlayerEvents.DroppedAmmo -= OnDroppedAmmo;
        }

        private void OnDroppedAmmo(PlayerDroppedAmmoEventArgs ev)
        {
            if (ev.Player == Player)
            {
                ev.AmmoPickup.Destroy();
                ev.Player.SetAmmo(ev.Type, 1);
            }
        }

        protected void OnReloadingWeapon(PlayerReloadingWeaponEventArgs ev)
        {
            if (ev.Player != Player || Player.CurrentItem == null || Player.CurrentItem is not FirearmItem item || !item.Base.TryGetModule(out IPrimaryAmmoContainerModule mod) || !item.Base.TryGetModule(out IReloaderModule mod2) || mod2.IsReloadingOrUnloading)
                return;

            Player.SetAmmo(mod.AmmoType, (ushort)(mod.AmmoMax - mod.AmmoStored + 1));
        }
    }
}
