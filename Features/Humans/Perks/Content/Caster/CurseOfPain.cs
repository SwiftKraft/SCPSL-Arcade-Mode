using LabApi.Features.Wrappers;
using SwiftArcadeMode.Utils.Effects;
using SwiftArcadeMode.Utils.Structures;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class CurseOfPain : SpellBase
    {
        public static readonly LayerMask ObstacleMask = LayerMask.GetMask("Default", "Door", "Glass");

        public override string Name => "Curse of Pain";

        public override Color BaseColor => new(0.5f, 0f, 0f);

        public override int RankIndex => 1;

        public override float CastTime => 0.4f;

        public override void Cast()
        {
            Caster.Player.Damage(10f, "Out of Blood.");

            Player target = GetClosestVisiblePlayer(Caster.Player.Camera.position, Caster.Player.Camera.forward, 10f, 25f, ObstacleMask, Caster.Player);
            target?.AddCustomEffect(new Effect(10f, this));
        }

        public static Player GetClosestVisiblePlayer(Vector3 origin, Vector3 direction, float maxDistance, float angle, int obstacleMask, Player self = null)
        {
            Player closestPlayer = null;
            float closestDist = float.MaxValue;
            float halfAngle = angle * 0.5f;

            foreach (Player target in Player.List)
            {
                if (!target.IsAlive || target == self)
                    continue;

                Vector3 toTarget = target.Position - origin;
                float distance = toTarget.magnitude;

                if (distance > maxDistance)
                    continue;

                if (Vector3.Angle(direction, toTarget) > halfAngle)
                    continue;

                if (Physics.Raycast(origin, toTarget.normalized, out RaycastHit hit, distance, obstacleMask))
                {
                    if (hit.collider != null && hit.collider.gameObject == target.GameObject)
                    {
                        if (distance < closestDist)
                        {
                            closestDist = distance;
                            closestPlayer = target;
                        }
                    }
                }
            }

            return closestPlayer;
        }

        public class Effect(float duration) : CustomEffectBase(duration)
        {
            public override int StackCount => 1;

            public readonly CurseOfPain ParentSpell;

            public readonly Timer Ticker = new(0.5f);

            public Effect(float duration, CurseOfPain spell) : this(duration) => ParentSpell = spell;

            public override void Add() => Parent.Player.Damage(20f * (Parent.Player.IsSCP ? 3f : 1f), "Cursed to death.");

            public override void Remove() { }

            public override void Tick()
            {
                Ticker.Tick(Time.fixedDeltaTime);

                if (Ticker.Ended)
                {
                    Ticker.Reset();
                    ParentSpell.Caster.Player.Heal(2f);
                    Parent.Player.Damage(2f * (Parent.Player.IsSCP ? 3f : 1f), "Cursed to death.");
                }
            }
        }
    }
}
