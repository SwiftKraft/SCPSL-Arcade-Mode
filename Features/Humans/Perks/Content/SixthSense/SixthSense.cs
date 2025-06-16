using LabApi.Features.Wrappers;
using SwiftUHC.Utils.Extensions;
using SwiftUHC.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReflectionExtensions = SwiftUHC.Utils.Extensions.ReflectionExtensions;

namespace SwiftUHC.Features.Humans.Perks.Content.SixthSense
{
    [Perk("SixthSense", Rarity.Uncommon)]
    public class SixthSense(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public static HashSet<Type> SenseCache
        {
            get
            {
                _senseCache ??= ReflectionExtensions.GetAllNonAbstractSubclasses<SenseBase>();
                return _senseCache;
            }
        }

        private static HashSet<Type> _senseCache;

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

        public static void RegisterSenses()
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            List<Type> types = callingAssembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(SenseBase).IsAssignableFrom(t))
                .ToList();

            foreach (Type t in types)
                if (!SenseCache.Contains(t))
                    SenseCache.Add(t);
        }
    }

    public abstract class SenseBase(SixthSense parent) : IWeight
    {
        public SixthSense Parent { get; private set; } = parent;

        public Player Player => Parent.Player;

        public virtual int Weight => 1;

        public abstract string Message();
    }
}
