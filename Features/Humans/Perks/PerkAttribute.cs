using SwiftUHC.Utils.Interfaces;
using System;

namespace SwiftUHC.Features.Humans.Perks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PerkAttribute(string id, Rarity rarity = Rarity.Common, params Type[] conflictPerks) : Attribute, IWeight
    {
        public Type Perk { get; set; }

        public readonly Type[] Conflicts = conflictPerks;

        public readonly string ID = id;

        public readonly Rarity Rarity = rarity;

        public int Weight => (int)Rarity;
    }
}
