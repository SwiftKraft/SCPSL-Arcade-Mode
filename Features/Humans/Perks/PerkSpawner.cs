using Hints;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MapGeneration;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftUHC.Features.Humans.Perks
{
    public static class PerkSpawner
    {
        public static readonly Dictionary<ushort, PerkAttribute> PerkPickups = [];
        public static readonly Dictionary<ushort, LightSourceToy> LightSources = [];

        public static void Enable()
        {
            ServerEvents.RoundStarted += OnRoundStarted;
            ServerEvents.WaveRespawned += OnWaveRespawned;
            ServerEvents.GeneratorActivated += OnGeneratorActivated;
            PlayerEvents.PickedUpItem += OnPickedUpItem;
            PlayerEvents.SearchingPickup += OnSearchingPickup;
        }

        public static void Disable()
        {
            ServerEvents.RoundStarted -= OnRoundStarted;
            ServerEvents.WaveRespawned -= OnWaveRespawned;
            ServerEvents.GeneratorActivated -= OnGeneratorActivated;
            PlayerEvents.PickedUpItem -= OnPickedUpItem;
            PlayerEvents.SearchingPickup -= OnSearchingPickup;
        }

        private static void OnGeneratorActivated(GeneratorActivatedEventArgs ev) => SpawnPerks();

        private static void OnSearchingPickup(PlayerSearchingPickupEventArgs ev)
        {
            if (!PerkPickups.ContainsKey(ev.Pickup.Serial))
                return;

            Type type = PerkPickups[ev.Pickup.Serial].Perk;
            PerkManager.PerkProfile prof = PerkPickups[ev.Pickup.Serial].Profile;
            ev.Player.SendHint($"Picking Up Perk: {prof.FancyName}\n{prof.Description}{(PerkManager.HasPerk(ev.Player, type) ? "\n\n<color=#FF0000><b>WARNING: Picking this up will remove the perk of the same type.</b></color>" : "")}", [HintEffectPresets.FadeOut()], 5f);
        }

        private static void OnPickedUpItem(PlayerPickedUpItemEventArgs ev)
        {
            if (!PerkPickups.ContainsKey(ev.Item.Serial))
                return;

            ev.Player.RemoveItem(ev.Item);

            if (!PerkManager.GivePerk(ev.Player, PerkPickups[ev.Item.Serial]))
            {
                SpawnPerk(PerkPickups[ev.Item.Serial], ev.Player.Position);
                return;
            }

            PerkPickups.Remove(ev.Item.Serial);
            if (LightSources.ContainsKey(ev.Item.Serial))
            {
                LightSources[ev.Item.Serial].Destroy();
                LightSources.Remove(ev.Item.Serial);
            }
        }

        private static void OnRoundStarted() => SpawnPerks();

        private static void OnWaveRespawned(WaveRespawnedEventArgs ev) => SpawnPerks();

        public static RoomName[] SpawnRooms = [
            RoomName.Lcz914,
            RoomName.Lcz173,
            RoomName.LczGlassroom,
            RoomName.LczArmory,
            RoomName.EzIntercom,
            RoomName.EzRedroom,
            RoomName.Hcz096,
            RoomName.Hcz127,
            RoomName.HczServers
            ];

        public static void SpawnPerks()
        {
            foreach (Room r in Room.List)
            {
                if ((r == null || Random.Range(0f, 1f) > Mathf.Lerp(0.3f, 0.6f, Mathf.InverseLerp(5, 25, Server.PlayerCount))) && !SpawnRooms.Contains(r.Name))
                    continue;

                SpawnPerk(PerkManager.GetRandomPerk((p) => p.Restriction == PerkRestriction.None), r.Position + Vector3.up * 2f);
            }
        }

        public static Pickup SpawnPerk(PerkAttribute ty, Vector3 location)
        {
            Pickup p = Pickup.Create(ItemType.Coin, location, Quaternion.identity, new(4f, 4f, 4f));
            if (p != null)
            {
                PerkPickups.Add(p.Serial, ty);
                p.Spawn();

                LightSourceToy toy = LightSourceToy.Create(p.Transform, true);
                toy.Intensity = 0.5f;
                if (ColorUtility.TryParseHtmlString(ty.Rarity.GetColor(), out Color col))
                    toy.Color = col;
                LightSources.Add(p.Serial, toy);
            }
            return p;
        }
    }
}
