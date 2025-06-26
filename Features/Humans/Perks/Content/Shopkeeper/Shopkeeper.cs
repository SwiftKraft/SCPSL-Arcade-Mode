﻿using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MapGeneration;
using SwiftUHC.Utils.Structures;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content.Shopkeeper
{
    [Perk("Shopkeeper", Rarity.Legendary)]
    public class Shopkeeper(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public static readonly Dictionary<Room, Player> ClaimStatus = [];

        public override string Name => "Shopkeeper | Shop Level " + ShopLevel;

        public override string PerkDescription => "Claim an entrance checkpoint to trade items. \nRestocks when you are in the room.";

        public int CustomerCount => Player.List.Count((p) => p.Room == Shop);

        public virtual ShopElement[] PresetElements => [
            new ShopItem(new(-5.5f, 1f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-4.5f, 1f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-3.5f, 1f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(0.72f, 0.8f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(1.84f, 0.8f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-1.37f, 0.8f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-0.4f, 0.8f, -5.25f), ShopItem.PresetTiers),
            new ShopItem(new(-2.22f, 0.3f, -2.59f), ShopItem.PresetTiers),
            new ShopItem(new(-2.22f, 0.3f, -4.13f), ShopItem.PresetTiers),
            ];

        public ShopElement[] Elements { get; private set; }

        public int ShopExperience
        {
            get => shopExperience;
            set
            {
                if (shopExperience == value)
                    return;

                shopExperience = value;

                while (shopExperience >= RequiredExperience)
                {
                    shopExperience -= RequiredExperience;
                    ShopLevel++;
                }
            }
        }

        public int ShopLevel
        {
            get => shopLevel;
            set
            {
                if (shopLevel == value)
                    return;

                shopLevel = value;
                SendMessage("Shop is now Level " + shopLevel);
                Inventory.OnPerksUpdated();
            }
        }

        public int RequiredExperience => ShopLevel * Mathf.Max(Server.PlayerCount / 2, 4);

        public override string ReadyMessage => Shop == null ? "No shop found, please claim a shop." : CanRestock ? "Restocking..." : "Failed to restock, player is not in shop room.";

        public virtual bool CanRestock => Shop != null && Player.Room == Shop;

        public override float Cooldown => Mathf.Clamp(45f / Mathf.Max(CustomerCount / 5, 1), 5f, Mathf.Infinity);

        readonly Timer claimTimer = new(10f);
        private int shopExperience;
        private int shopLevel = 1;

        public Room Shop { get; private set; }

        public override void Init()
        {
            base.Init();
            PlayerEvents.Death += OnDeath;
            PlayerEvents.ChangedRole += OnChangedRole;

            claimTimer.Reset();
        }

        private void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            UnclaimRoom();
        }

        private void OnDeath(LabApi.Events.Arguments.PlayerEvents.PlayerDeathEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            UnclaimRoom();
        }

        public override void Effect() => Restock();

        public override void Tick()
        {
            base.Tick();

            if (Shop != null)
                return;

            Room curRoom = Player.Room;
            if (curRoom.Name == RoomName.HczCheckpointToEntranceZone && curRoom.Zone == FacilityZone.HeavyContainment && (!ClaimStatus.ContainsKey(curRoom) || ClaimStatus[curRoom] == null))
            {
                claimTimer.Tick(Time.fixedDeltaTime);

                if (claimTimer.Ended)
                    ClaimRoom(curRoom);
                else
                    SendMessage("Claiming checkpoint in: " + Mathf.Round(claimTimer.CurrentValue) + "s");
            }
            else
                claimTimer.Reset();
        }

        public void ClaimRoom(Room room)
        {
            UnclaimRoom();

            if (!ClaimStatus.ContainsKey(room))
                ClaimStatus.Add(room, Player);
            else
                ClaimStatus[room] = Player;

            Elements = PresetElements;
            Shop = room;

            foreach (ShopElement element in Elements)
                element.Init(this);

            SendMessage("Claimed checkpoint! Stay in the room to restock your shop.");
        }

        public void UnclaimRoom()
        {
            if (Shop == null)
                return;

            if (ClaimStatus.ContainsKey(Shop))
            {
                foreach (ShopElement element in Elements)
                    element.Remove();

                Elements = null;
                ClaimStatus[Shop] = null;
                Shop = null;
                SendMessage("You have lost your shop. ");
                claimTimer.Reset();
            }
        }

        public void Restock()
        {
            if (!CanRestock)
                return;

            foreach (ShopElement element in Elements)
                element?.Restock();
        }

        public override void Remove()
        {
            base.Remove();

            PlayerEvents.Death -= OnDeath;
            PlayerEvents.ChangedRole -= OnChangedRole;
            UnclaimRoom();
        }
    }
}
