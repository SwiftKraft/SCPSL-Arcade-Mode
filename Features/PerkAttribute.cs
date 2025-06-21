using SwiftUHC.Utils.Interfaces;
using System;
using System.Linq;

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

        public int Weight => (int)Rarity;

        public PerkManager.PerkProfile Profile;

        public bool HasConflicts(PerkInventory perks, out PerkBase perk)
        {
            for (int i = 0; i < Conflicts.Length; i++)
                foreach (var v in perks.Perks.Where(v => v.GetType() == Conflicts[i]))
                {
                    perk = v;
                    return true;
                }

            perk = null;
            return false;
        }
    }

    public enum PerkRestriction
    {
        None,
        SCP
    }
}
