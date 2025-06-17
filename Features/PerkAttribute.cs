using SwiftUHC.Utils.Interfaces;
using System;

namespace SwiftUHC.Features
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PerkAttribute(string id, Rarity rarity = Rarity.Common, PerkRestriction restriction = PerkRestriction.None, params Type[] conflictPerks) : Attribute, IWeight
    {
        public Type Perk { get; set; }

        public readonly Type[] Conflicts = conflictPerks;

        public readonly string ID = id;

        public readonly Rarity Rarity = rarity;

        public readonly PerkRestriction Restriction = restriction;

        public PerkManager.PerkProfile Profile;

        public int Weight => (int)Rarity;

        public bool HasConflicts(PerkInventory perks, out PerkBase perk)
        {
            for (int i = 0; i < Conflicts.Length; i++)
                for (int j = 0; j < perks.Perks.Count; j++)
                    if (perks.Perks[j].GetType().BaseType == Conflicts[i])
                    {
                        perk = perks.Perks[j];
                        return true;
                    }
            perk = null;
            return false;
        }
    }

    public enum PerkRestriction
    {
        None,
        Human,
        SCP
    }
}
