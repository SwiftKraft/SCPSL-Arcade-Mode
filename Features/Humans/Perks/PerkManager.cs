using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SwiftUHC.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SwiftUHC.Features.Humans.Perks
{
    public static class PerkManager
    {
        public struct PerkProfile(Rarity r, string name, string desc)
        {
            public Rarity Rarity = r;
            public string Name = name;
            public string Description = desc;
        }

        public static readonly Dictionary<Player, PerkInventory> Inventories = [];
        public static readonly Dictionary<string, PerkAttribute> RegisteredPerks = [];
        public static readonly Dictionary<Type, PerkProfile> Profiles = [];

        public static void Enable()
        {
            FindPerks();

            PlayerEvents.Death += OnPlayerDeath;
            ServerEvents.RoundRestarted += OnRoundRestarted;
        }

        public static void Disable()
        {
            PlayerEvents.Death -= OnPlayerDeath;
            ServerEvents.RoundRestarted -= OnRoundRestarted;
        }

        private static void OnRoundRestarted() => Inventories.Clear();

        private static void OnPlayerDeath(PlayerDeathEventArgs ev)
        {
            if (Inventories.ContainsKey(ev.Player))
                Inventories[ev.Player].RemoveRandom();
        }

        public static void FindPerks()
        {
            Dictionary<Type, PerkAttribute> atts = AppDomain.CurrentDomain
    .GetAssemblies()
    .SelectMany(a =>
    {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
    })
    .Select(t => (type: t, attr: t.GetCustomAttribute<PerkAttribute>()))
    .Where(pair => pair.attr != null)
    .ToDictionary(pair => pair.type, pair => pair.attr);

            foreach (KeyValuePair<Type, PerkAttribute> attr in atts)
            {
                attr.Value.Perk = attr.Key;
                RegisteredPerks.Add(attr.Value.ID.ToLower(), attr.Value);
                PerkBase p = (PerkBase)Activator.CreateInstance(attr.Key, new PerkInventory(null));
                Profiles.Add(attr.Key, new(attr.Value.Rarity, p.Name, p.Description));
            }
        }

        public static PerkAttribute GetRandomPerk() => RegisteredPerks.Values.ToArray().GetWeightedRandom();

        public static PerkAttribute GetPerk(string id) => RegisteredPerks.ContainsKey(id) ? RegisteredPerks[id] : null;

        public static bool TryGetPerk(string id, out PerkAttribute t)
        {
            t = GetPerk(id);
            return t != null;
        }

        public static bool GivePerk(Player p, Type t)
        {
            if (!Inventories.ContainsKey(p))
                Register(p);

            return Inventories[p].AddPerk(t);
        }

        public static void RemovePerk(Player p, Type t)
        {
            if (!Inventories.ContainsKey(p))
            {
                Register(p);
                return;
            }

            Inventories[p].RemovePerk(t);
        }

        public static bool HasPerk(Player p, Type perk) => Inventories.ContainsKey(p) && Inventories[p].HasPerk(perk);

        public static void Register(Player p)
        {
            if (p == null || Inventories.ContainsKey(p))
                return;

            Inventories.Add(p, new(p));
        }

        public static void Remove(Player p)
        {
            if (p != null)
                Inventories.Remove(p);
        }

        public static void Tick()
        {
            foreach (PerkInventory inv in Inventories.Values)
                inv?.Tick();
        }
    }
}
