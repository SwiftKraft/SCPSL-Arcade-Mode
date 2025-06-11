using Hints;
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

        public int Limit = 5;

        public bool AddPerk(PerkAttribute type)
        {
            if (type == null || type.Perk.IsAbstract || (type.Perk != typeof(PerkBase) && !type.Perk.IsSubclassOf(typeof(PerkBase))))
                return false;

            PerkManager.PerkProfile prof = type.Profile;

            if (type.HasConflicts(this, out PerkBase conf))
            {
                Parent.SendHint($"{prof.FancyName} conflicts with {conf.FancyName}!", [HintEffectPresets.FadeOut()], 5f);
                return false;
            }

            PerkBase perk = Perks.FirstOrDefault((p) => p.GetType() == type.Perk);
            
            if (perk != null)
            {
                RemovePerk(perk);
                return true;
            }

            if (Perks.Count >= Limit)
                return false;

            PerkBase p = (PerkBase)Activator.CreateInstance(type.Perk, this);
            p.Rarity = type.Rarity;
            Perks.Add(p);
            p.Init();
            Parent.SendHint($"Acquired Perk ({Perks.Count}/{Limit}): {prof.FancyName}\n{prof.Description}\n\nPress \"~\" and type \".sp\" to see what perks you have!", [HintEffectPresets.FadeOut()], 10f);
            return true;
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

        public bool HasPerk(Type t) => Perks.FirstOrDefault((p) => p.GetType() == t) != null;

        public void ClearPerks() => Perks.Clear();

        public void RemovePerk(PerkBase perk)
        {
            if (perk == null)
                return;

            Perks.Remove(perk);
            perk.Remove();
            Parent.SendHint($"Removed Perk: {perk.FancyName}\n\nPress \"~\" and type \".sp\" to see what perks you have!", [HintEffectPresets.FadeOut()], 10f);
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
            if (!Parent.IsAlive)
                return;

            for (int i = 0; i < Perks.Count; i++)
                Perks[i]?.Tick();
        }
    }
}
