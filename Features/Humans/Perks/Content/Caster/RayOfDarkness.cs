﻿using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Projectiles;
using SwiftArcadeMode.Utils.Structures;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class RayOfDarkness : SpellBase
    {
        public static readonly LayerMask CastMask = LayerMask.GetMask("Default", "Door", "Glass", "Hitbox");

        public PrimitiveObjectToy RayVisual;

        public override string Name => "Ray of Darkness";

        public override Color BaseColor => Color.black;

        public override int RankIndex => 3;

        public override float CastTime => 1f;

        public override float CastDuration => 3f;

        Vector3 hitPos;

        readonly Timer timer = new(0.05f);

        public override void Cast()
        {
            PlaySound("cast");

            ShootRay();
            RayVisual = PrimitiveObjectToy.Create(null, false);
            RayVisual.Type = PrimitiveType.Cylinder;
            RayVisual.Color = new(0f, 0f, 0f, 0.9f);
            RayVisual.Flags = AdminToys.PrimitiveFlags.Visible;
            RayVisual.SyncInterval = 0;
            RayVisual.MovementSmoothing = 1;
            UpdateRay();
            RayVisual.Spawn();
        }

        public override void Tick()
        {
            base.Tick();

            UpdateRay();

            timer.Tick(Time.fixedDeltaTime);
            if (timer.Ended)
            {
                ShootRay();
                timer.Reset();

                if (!Caster.Player.IsAlive)
                {
                    RayVisual?.Destroy();
                    return;
                }
            }
        }

        public override void End()
        {
            base.End();
            RayVisual?.Destroy();
        }

        public void UpdateRay() => StretchBetween(RayVisual, Caster.Player.Camera.position + (Caster.Player.Camera.rotation * new Vector3(0.1f, -0.1f, 0.1f)), hitPos, Mathf.Sin(Time.time * 64f) * 0.005f + 0.05f);

        public void ShootRay()
        {
            if (!Caster.Player.IsAlive)
                return;

            Vector3 origin = Caster.Player.Camera.position;
            Vector3 direction = Caster.Player.Camera.forward;
            float maxDistance = 20f;

            RaycastHit[] hits = Physics.SphereCastAll(origin, 0.1f, direction, maxDistance, CastMask, QueryTriggerInteraction.Ignore);

            if (hits.Length == 0)
            {
                hitPos = origin + direction * maxDistance;
                return;
            }

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            RaycastHit? validHit = null;

            foreach (var hit in hits)
            {
                Transform t = hit.transform;
                if (t == Caster.Player.GameObject.transform || t.IsChildOf(Caster.Player.GameObject.transform))
                    continue;

                validHit = hit;
                break;
            }

            if (!validHit.HasValue)
            {
                hitPos = origin + direction * maxDistance;
                return;
            }

            var _hit = validHit.Value;
            hitPos = _hit.point;

            LightSourceToy toy = LightSourceToy.Create(hitPos, null, false);
            toy.Color = Color.white;
            toy.Intensity = 0.4f;
            LightExplosion.Create(toy, 15f);

            if (_hit.collider.transform.TryGetComponentInParent(out ReferenceHub hub) && hub != Caster.Player.ReferenceHub)
            {
                hub.playerStats.DealDamage(
                    new ExplosionDamageHandler(
                        new(Caster.Player.ReferenceHub),
                        direction,
                        6f * (hub.IsSCP() ? 2f : 1f),
                        100,
                        ExplosionType.Disruptor
                    )
                );
                Caster.Player.SendHitMarker(0.5f);
            }
        }

        public static void StretchBetween(PrimitiveObjectToy t, Vector3 start, Vector3 end, float radius)
        {
            if (t == null)
                return;

            Vector3 direction = end - start;
            float length = direction.magnitude;

            if (length <= 0.0001f)
                return;

            direction.Normalize();

            t.Position = (start + end) * 0.5f;
            t.Rotation = Quaternion.FromToRotation(Vector3.up, direction);
            t.Scale = new Vector3(radius * 2f, length * 0.5f, radius * 2f);
        }
    }
}
