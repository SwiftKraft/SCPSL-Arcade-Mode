using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Effects;
using SwiftArcadeMode.Utils.Structures;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class CurseOfPain : SpellBase
    {
        public static readonly LayerMask CastMask = LayerMask.GetMask("Default", "Door", "Glass", "Hitbox");

        public override string Name => "Curse of Pain";

        public override Color BaseColor => new(0.5f, 0f, 0f);

        public override int RankIndex => 1;

        public override float CastTime => 0.5f;

        public override void Cast()
        {
            Caster.Player.Damage(10f, "Out of Blood.");

            if (Physics.Raycast(Caster.Player.Camera.position, Caster.Player.Camera.forward, out RaycastHit _hit, 10f, CastMask, QueryTriggerInteraction.Ignore) && _hit.collider.transform.TryGetComponentInParent(out ReferenceHub hub))
                Player.Get(hub)?.AddCustomEffect(new Effect(10f, this));
        }

        public class Effect(float duration) : CustomEffectBase(duration)
        {
            LightSourceToy light;

            public override int StackCount => 1;

            public readonly CurseOfPain ParentSpell;

            public readonly Timer Ticker = new(0.5f, false);

            public Effect(float duration, CurseOfPain spell) : this(duration) => ParentSpell = spell;

            public override void Add()
            {
                Parent.Player.Damage(20f * (Parent.Player.IsSCP ? 3f : 1f), ParentSpell.Caster.Player, default, 100);
                ParentSpell.Caster.Player.SendHitMarker(1.5f);

                if (Parent.Player.IsAlive)
                {
                    light = LightSourceToy.Create(Parent.Player.Position, null, false);
                    light.Color = Color.red;
                    light.Intensity = 1f;
                    light.SyncInterval = 0f;
                    light.ShadowType = LightShadows.None;
                    light.Spawn();
                }
                else
                    ParentSpell.Caster.Player.Heal(30f);
            }

            public override void Remove() => light?.Destroy();

            public override void Tick()
            {
                Ticker.Tick(Time.fixedDeltaTime);

                if (Ticker.Ended)
                {
                    Ticker.Reset();
                    ParentSpell.Caster.Player.Heal(2f);
                    Parent.Player.Damage(2f * (Parent.Player.IsSCP ? 3f : 1f), ParentSpell.Caster.Player, default, 100);
                    ParentSpell.Caster.Player.SendHitMarker(0.3f);
                }

                if (light != null)
                {
                    light.Position = Parent.Player.Position;
                    light.Intensity = (Mathf.Sin(Time.time * 4f) + 1.5f) * 0.5f;
                }
            }
        }
    }
}