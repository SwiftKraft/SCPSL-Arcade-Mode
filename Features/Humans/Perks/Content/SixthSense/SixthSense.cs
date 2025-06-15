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

        public override string Name => "Sixth Sense";

        public override string Description => "Provides obscure, but useful information regarding players and enemies.";

        public override float Cooldown => 15f;

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
