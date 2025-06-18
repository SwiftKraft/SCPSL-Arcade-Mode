﻿using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp049Events;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using System.Collections.Generic;

namespace SwiftUHC.Features.SCPs.Upgrades.Content.SCP049.ArmyBuilder
{
    [Perk("049.ArmyBuilder", Rarity.Uncommon, PerkRestriction.SCP)]
    public class ArmyBuilder(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public readonly List<Player> OwnedZombies = [];

        public override Type[] AllUpgrades => [
            typeof(EfficientReanimation),
            typeof(Infector),
            ];

        public override string Name => "Army Builder";

        public override string Description => "Focuses on zombie buffs.";

        public event Action<Player> OnAddedZombie;
        public event Action<Player> OnLostZombie;
        public event Action<Player> OnZombieKilled;

        public override void Init()
        {
            base.Init();
            Scp049Events.ResurrectedBody += OnResurrected;
            PlayerEvents.ChangedRole += OnChangedRole;
            PlayerEvents.Dying += OnDying;
        }

        private void OnDying(PlayerDyingEventArgs ev)
        {
            if (ev.Player.Role != RoleTypeId.Scp0492 || !OwnedZombies.Contains(ev.Player))
                return;

            OnZombieKilled?.Invoke(ev.Player);
        }

        private void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            if (ev.OldRole == ev.NewRole.RoleTypeId || ev.OldRole != RoleTypeId.Scp0492 || !OwnedZombies.Contains(ev.Player))
                return;

            OnLostZombie?.Invoke(ev.Player);
            OwnedZombies.Remove(ev.Player);
            SendMessage("Lost Zombie: " + ev.Player.DisplayName);
        }

        private void OnResurrected(Scp049ResurrectedBodyEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            AddZombie(ev.Target);
        }

        public void AddZombie(Player p)
        {
            OnAddedZombie?.Invoke(p);
            OwnedZombies.Add(p);
            SendMessage("Added Zombie: " + p.DisplayName);
        }

        public override void Remove()
        {
            base.Remove();
            Scp049Events.ResurrectedBody -= OnResurrected;
            PlayerEvents.ChangedRole -= OnChangedRole;
            PlayerEvents.Dying -= OnDying;
        }
    }
}
