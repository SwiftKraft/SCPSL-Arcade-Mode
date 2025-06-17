using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SwiftUHC.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SwiftUHC.Features
{
    public static class PerkManager
    {
        public const string PerkNameSpace = "base";

        public struct PerkProfile(Rarity r, string name, string desc)
        {
            public Rarity Rarity = r;
            public string Name = name;
            public string Description = desc;

            public readonly string FancyName => Name.FancifyPerkName(Rarity);
        }

        public static readonly Dictionary<Player, PerkInventory> Inventories = [];
        public static readonly Dictionary<string, PerkAttribute> RegisteredPerks = [];

        public static void Enable()
        {
            RegisterPerks(PerkNameSpace);

            PlayerEvents.Death += OnPlayerDeath;
            PlayerEvents.ChangedSpectator += OnChangedSpectator;
            PlayerEvents.ChangedRole += OnChangedRole;
            ServerEvents.RoundRestarted += OnRoundRestarted;
        }

        public static void Disable()
        {
            PlayerEvents.Death -= OnPlayerDeath;
            PlayerEvents.ChangedSpectator -= OnChangedSpectator;
            PlayerEvents.ChangedRole -= OnChangedRole;
            ServerEvents.RoundRestarted -= OnRoundRestarted;
        }

        private static void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            ev.Player.SendHint("", 1f);

            if (Inventories.ContainsKey(ev.Player) && Inventories[ev.Player].Perks.Count > 0)
            {
                List<PerkBase> perks = [];
                foreach (PerkBase p in Inventories[ev.Player].Perks)
                    if (ev.Player.IsAlive && p.Restriction != PerkRestriction.None && ((ev.Player.IsSCP && p.Restriction != PerkRestriction.SCP) || (ev.Player.IsHuman && p.Restriction != PerkRestriction.Human)))
                        perks.Add(p);
                if (perks.Count > 0)
                {
                    foreach (PerkBase p in perks)
                        Inventories[ev.Player].RemovePerk(p);

                    ev.Player.SendHint("Removed " + perks.Count + " perks, because of role incompatibility.", 5f);
                }
            }
        }

        private static void OnChangedSpectator(PlayerChangedSpectatorEventArgs ev)
        {
            if (ev.NewTarget == null)
                return;

            if (!Inventories.ContainsKey(ev.NewTarget))
            {
                ev.Player.SendHint("", 1f);
                return;
            }

            StringBuilder builder = new($"\n\n\n{ev.NewTarget.DisplayName}'s Perks\n");

            foreach (PerkBase perk in Inventories[ev.NewTarget].Perks)
                builder.AppendLine($"- {perk.FancyName}");

            ev.Player.SendHint(builder.ToString(), 60f);
        }

        private static void OnRoundRestarted() => Inventories.Clear();

        private static void OnPlayerDeath(PlayerDeathEventArgs ev)
        {
            if (Inventories.ContainsKey(ev.Player) && Inventories[ev.Player].Perks.Count > 0)
                Inventories[ev.Player].RemoveRandom();
        }

        public static string FancifyPerkName(this string perkName, Rarity rarity) => $"<color={rarity.GetColor()}><b>{perkName}</b></color>";

        public static void RegisterPerks(string nameSpace)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            Dictionary<Type, PerkAttribute> atts = callingAssembly
                .GetTypes()
                .Select(t => (type: t, attr: t.GetCustomAttribute<PerkAttribute>()))
                .Where(pair => pair.attr != null)
                .ToDictionary(pair => pair.type, pair => pair.attr);

            foreach (KeyValuePair<Type, PerkAttribute> attr in atts)
            {
                attr.Value.Perk = attr.Key;
                RegisteredPerks.Add((RegisteredPerks.ContainsKey(attr.Value.ID) ? nameSpace.ToLower() + "." : "") + attr.Value.ID.ToLower(), attr.Value);
                PerkBase p = (PerkBase)Activator.CreateInstance(attr.Key, new PerkInventory(null));
                attr.Value.Profile = new(attr.Value.Rarity, p.Name, p.Description);
            }
        }

        public static PerkAttribute GetRandomPerk() => RegisteredPerks.Values.ToArray().GetWeightedRandom();

        public static PerkAttribute GetPerk(string id) => RegisteredPerks.ContainsKey(id) ? RegisteredPerks[id] : null;

        public static bool TryGetPerk(string id, out PerkAttribute t)
        {
            t = GetPerk(id);
            return t != null;
        }

        public static bool GivePerk(Player p, PerkAttribute t)
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
