using LabApi.Features.Wrappers;
using SwiftUHC.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.NonAllocLINQ;

namespace SwiftUHC.Features.Humans.Perks
{
    public class PerkInventory(Player targetPlayer)
    {
        public readonly Player Parent = targetPlayer;
        public readonly List<PerkBase> Perks = [];

        public void AddPerk(Type type)
        {
            if (type == null || type.IsAbstract || (type != typeof(PerkBase) && !type.IsSubclassOf(typeof(PerkBase))))
                return;

            PerkBase perk = Perks.FirstOrDefault((p) => p.GetType() == type);
            
            if (perk != null)
            {
                Perks.Remove(perk);
                perk.Remove();
            }

            PerkBase p = (PerkBase)Activator.CreateInstance(type, this);
            p.Rarity = PerkManager.Profiles.ContainsKey(type) ? PerkManager.Profiles[type].Rarity : Rarity.Secret;
            Perks.Add(p);
            p.Init();
        }

        public void RemovePerk(Type type)
        {
            if (type == null)
                return;

            PerkBase perk = Perks.FirstOrDefault((p) => p.GetType() == type);

            if (perk == null)
                return;

            RemovePerk(perk);
        }

        public void ClearPerks() => Perks.Clear();

        public void RemovePerk(PerkBase perk)
        {
            if (perk == null)
                return;

            Perks.Remove(perk);
            perk.Remove();
            Parent.SendBroadcast($"{perk.Name} has been removed from you!", 5, Broadcast.BroadcastFlags.Normal, false);
        }

        public PerkBase RemoveRandom()
        {
            if (Perks.Count <= 0)
                return null;

            PerkBase perk = Perks.GetRandom();
            RemovePerk(perk);
            return perk;
        }

        public void Tick()
        {
            for (int i = 0; i < Perks.Count; i++)
                Perks[i]?.Tick();
        }
    }
}
