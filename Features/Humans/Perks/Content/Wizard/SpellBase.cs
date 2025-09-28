using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Wizard
{
    public abstract class SpellBase
    {
        public Wizard Wizard { get; private set; }

        public abstract string Name { get; }
        public abstract Color BaseColor { get; }
        public abstract Color AccentColor { get; }
        public abstract int RankIndex { get; }
        public abstract float CastTime { get; }

        public virtual void Init(Wizard wiz) => Wizard = wiz;
        public abstract void Cast();
    }
}
