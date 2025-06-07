using SwiftUHC.Utils.Interfaces;
using System;

namespace SwiftUHC.Features.Humans.Perks
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PerkAttribute(string id, Rarity rarity = Rarity.Common) : Attribute, IWeight
    {
        public Type Perk { get; set; }

        public string ID { get; private set; } = id;

        public Rarity Rarity { get; set; } = rarity;

        public int Weight => (int)Rarity;
    }
}
