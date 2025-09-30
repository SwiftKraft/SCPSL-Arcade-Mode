using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public abstract class SpellBase
    {
        public Caster Caster { get; private set; }

        public abstract string Name { get; }
        public abstract Color BaseColor { get; }
        public abstract Color AccentColor { get; }
        public abstract int RankIndex { get; }
        public abstract float CastTime { get; }

        public virtual void Init(Caster wiz) => Caster = wiz;
        public abstract void Cast();
    }
}
