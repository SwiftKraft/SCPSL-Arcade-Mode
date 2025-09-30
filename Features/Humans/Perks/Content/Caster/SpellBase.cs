using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public abstract class SpellBase
    {
        public CasterBase Caster { get; private set; }

        public abstract string Name { get; }
        public abstract Color BaseColor { get; }
        public abstract int RankIndex { get; }
        public abstract float CastTime { get; }

        public virtual void Init(CasterBase wiz) => Caster = wiz;
        public abstract void Cast();
    }
}
