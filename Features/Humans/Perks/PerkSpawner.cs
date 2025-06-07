using Hints;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftUHC.Features.Humans.Perks
{
    public static class PerkSpawner
    {
        public static readonly Dictionary<ushort, Type> PerkPickups = [];

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

            Type type = PerkPickups[ev.Pickup.Serial];
            PerkManager.PerkProfile prof = PerkManager.Profiles.ContainsKey(type) ? PerkManager.Profiles[type] : default;
            ev.Player.SendHint($"Picking Up Perk: <color={prof.Rarity.GetColor()}><b>{prof.Name}</b></color>\n{prof.Description}", [HintEffectPresets.FadeOut()], 5f);
        }

        private static void OnPickedUpItem(PlayerPickedUpItemEventArgs ev)
        {
            if (!PerkPickups.ContainsKey(ev.Item.Serial))
                return;

            PerkManager.GivePerk(ev.Player, PerkPickups[ev.Item.Serial]);
            ev.Player.RemoveItem(ev.Item);

            Type type = PerkPickups[ev.Item.Serial];
            PerkManager.PerkProfile prof = PerkManager.Profiles.ContainsKey(type) ? PerkManager.Profiles[type] : default;
            ev.Player.SendHint($"Acquired Perk: <color={prof.Rarity.GetColor()}><b>{prof.Name}</b></color>\n{prof.Description}\n\nPress \"~\" and type \".sp\" to see what perks you have!", [HintEffectPresets.FadeOut()], 10f);

            PerkPickups.Remove(ev.Item.Serial);
        }

        private static void OnRoundStarted() => SpawnPerks();

        private static void OnWaveRespawned(WaveRespawnedEventArgs ev) => SpawnPerks();

        public static void SpawnPerks()
        {
            foreach (Room r in Room.List)
            {
                if (r == null || Random.Range(0f, 1f) > 0.5f)
                    continue;

                Pickup p = Pickup.Create(ItemType.Coin, r.Position + Vector3.up * 2f, Quaternion.identity, new(4f, 4f, 4f));
                if (p != null)
                {
                    PerkPickups.Add(p.Serial, PerkManager.GetRandomPerk());
                    p.Spawn();
                }
            }
        }
    }
}
