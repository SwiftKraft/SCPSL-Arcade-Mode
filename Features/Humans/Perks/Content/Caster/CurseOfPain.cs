using SwiftArcadeMode.Utils.Effects;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class CurseOfPain : SpellBase
    {
        public override string Name => "Curse of Pain";

        public override Color BaseColor => new(0.5f, 0f, 0f);

        public override int RankIndex => 1;

        public override float CastTime => 0.4f;

        public override void Cast()
        {

        }

        public class Effect(float duration) : CustomEffectBase(duration)
        {
            public override void Add()
            {
                throw new System.NotImplementedException();
            }

            public override void Remove()
            {
                throw new System.NotImplementedException();
            }

            public override void Tick()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
