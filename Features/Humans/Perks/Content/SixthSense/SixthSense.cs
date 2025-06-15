using LabApi.Features.Wrappers;
using SwiftUHC.Utils.Extensions;
using SwiftUHC.Utils.Interfaces;
using System;
using System.Collections.Generic;

namespace SwiftUHC.Features.Humans.Perks.Content.SixthSense
{
    [Perk("SixthSense", Rarity.Uncommon)]
    public class SixthSense(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public static List<Type> SenseCache
        {
            get
            {
                _senseCache ??= ReflectionExtensions.GetAllNonAbstractSubclasses<SenseBase>();
                return _senseCache;
            }
        }

        private static List<Type> _senseCache;

        public readonly List<SenseBase> Senses = [];

        public override string ReadyMessage => Senses.GetRandom().Message();

        public override void Init()
        {
            base.Init();
            foreach (Type t in SenseCache)
                Senses.Add((SenseBase)Activator.CreateInstance(t, this));
        }

        public override void Effect() { }
    }

    public abstract class SenseBase(SixthSense parent) : IWeight
    {
        public SixthSense Parent { get; private set; } = parent;

        public Player Player => Parent.Player;

        public virtual int Weight => 1;

        public abstract string Message();
    }
}
