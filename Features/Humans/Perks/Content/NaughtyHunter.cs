using Hints;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("NaughtyHunter", Rarity.Epic)]
    public class NaughtyHunter(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public override string PerkDescription => "Gives you a naughty gun that recharges.\nShooting someone with this gun will send them to the\n<i>NAUGHTY ROOM</i>";

        public override string Name => "Naughty Hunter";

        public override string ReadyMessage => "Recharged!";

        public static Vector3 Location = new(-16, 315, -31);

        public override float Cooldown => 120f;

        public int Duration => 10;

        public ushort CurrentGun;

        public override void Init()
        {
            base.Init();

            GiveItem();

            PlayerEvents.Death += OnDeath;
            PlayerEvents.ReloadingWeapon += OnReloadingWeapon;
            PlayerEvents.UnloadingWeapon += OnUnloadingWeapon;
            PlayerEvents.Spawned += OnSpawned;
            PlayerEvents.Hurting += OnHurting;
            PlayerEvents.ChangedItem += OnChangedItem;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Death -= OnDeath;
            PlayerEvents.ReloadingWeapon -= OnReloadingWeapon;
            PlayerEvents.UnloadingWeapon -= OnUnloadingWeapon;
            PlayerEvents.Spawned -= OnSpawned;
            PlayerEvents.Hurting -= OnHurting;
            PlayerEvents.ChangedItem -= OnChangedItem;
        }

        private void OnChangedItem(LabApi.Events.Arguments.PlayerEvents.PlayerChangedItemEventArgs ev)
        {
            if (ev.NewItem != null && ev.NewItem.Serial == CurrentGun)
                ev.Player.SendHint("Equipped the NAUGHTY GUN.\nSends people to the NAUGHTY ROOM.\nCannot be reloaded or unloaded.", [HintEffectPresets.FadeOut()], 5f);
        }

        private void OnHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Attacker.CurrentItem == null || ev.Attacker.CurrentItem.Serial != CurrentGun)
                return;

            Timing.RunCoroutine(NaughtyCoroutine(ev.Player, ev.Player.Position));
        }

        private IEnumerator<float> NaughtyCoroutine(Player p, Vector3 original)
        {
            yield return Timing.WaitForSeconds(0.1f);
            p.Position = Location;
            for (int i = 0; i < Duration; i++)
            {
                p.SendHint($"You've been sent to the NAUGHTY ROOM.\n{Duration - i}s Left.", 2f);

                if (!p.IsAlive)
                    yield break;

                yield return Timing.WaitForSeconds(1f);
            }
            p.Position = original;
        }

        private void GiveItem()
        {
            if (!Player.IsInventoryFull)
            {
                Item it = Player.AddItem(ItemType.GunCOM15);
                CurrentGun = it.Serial;

                if (it.Base is Firearm f && f.TryGetModule(out IPrimaryAmmoContainerModule mod, false))
                    mod.ServerModifyAmmo(-mod.AmmoMax);
            }
        }

        private void OnSpawned(LabApi.Events.Arguments.PlayerEvents.PlayerSpawnedEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            GiveItem();
        }

        private void OnReloadingWeapon(LabApi.Events.Arguments.PlayerEvents.PlayerReloadingWeaponEventArgs ev)
        {
            if (ev.FirearmItem.Serial != CurrentGun)
                return;

            ev.IsAllowed = false;
        }

        private void OnUnloadingWeapon(LabApi.Events.Arguments.PlayerEvents.PlayerUnloadingWeaponEventArgs ev)
        {
            if (ev.FirearmItem.Serial != CurrentGun)
                return;

            ev.IsAllowed = false;
        }

        private void OnDeath(LabApi.Events.Arguments.PlayerEvents.PlayerDeathEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            CurrentGun = 0;
        }

        public override void Effect()
        {
            if (CurrentGun == 0 || !Item.TryGet(CurrentGun, out Item item) || item.Base is not Firearm firearm || !firearm.TryGetModule(out IPrimaryAmmoContainerModule mod, false) || mod.AmmoStored >= 1)
                return;

            mod.ServerModifyAmmo(1);

            if (firearm.TryGetModule(out AutomaticActionModule mod2, false))
            {
                mod2.BoltLocked = false;
                mod2.Cocked = true;
            }
        }
    }
}
