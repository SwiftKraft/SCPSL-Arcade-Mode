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

        public bool AddPerk(Type type)
        {
            if (type == null || type.IsAbstract || (type != typeof(PerkBase) && !type.IsSubclassOf(typeof(PerkBase))))
                return false;

            PerkManager.PerkProfile prof = PerkManager.Profiles.ContainsKey(type) ? PerkManager.Profiles[type] : default;

            PerkBase perk = Perks.FirstOrDefault((p) => p.GetType() == type);
            
            if (perk != null)
            {
                RemovePerk(perk);
                return true;
            }

            if (Perks.Count >= Limit)
                return false;

            PerkBase p = (PerkBase)Activator.CreateInstance(type, this);
            p.Rarity = PerkManager.Profiles.ContainsKey(type) ? PerkManager.Profiles[type].Rarity : Rarity.Secret;
            Perks.Add(p);
            p.Init();
            Parent.SendHint($"Acquired Perk ({Perks.Count}/{Limit}): <color={prof.Rarity.GetColor()}><b>{prof.Name}</b></color>\n{prof.Description}\n\nPress \"~\" and type \".sp\" to see what perks you have!", [HintEffectPresets.FadeOut()], 10f);
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
            Parent.SendHint($"Removed Perk: <color={perk.Rarity.GetColor()}><b>{perk.Name}</b></color>\n\nPress \"~\" and type \".sp\" to see what perks you have!", [HintEffectPresets.FadeOut()], 10f);
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
