using Footprinting;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Projectiles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class MagicMissile : SpellBase
    {
        public override string Name => "Magic Missile";

        public override Color BaseColor => Color.magenta;

        public override int RankIndex => 2;

        public override float CastTime => 0.5f;

        CoroutineHandle coroutine;

        public override void Cast()
        {
            new Projectile(Caster.Player.Camera.position, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 13f, 10f, Caster.Player);

            coroutine = Timing.CallPeriodically(1.6f, 0.2f, () =>
            {
                if (!Caster.Player.IsAlive)
                {
                    Timing.KillCoroutines(coroutine);
                    return;
                }

                new Projectile(Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 13f, 10f, Caster.Player);
            });
        }

        public class Projectile(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : CasterBase.MagicProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            const float homingRangeSqr = 25f;
            float initialSpeed;
            Player homing;

            List<Player> targets;

            public override PrimitiveObjectToy[] CreateBalls() => [PrimitiveObjectToy.Create(default, Quaternion.identity, new(0.2f, 0.2f, 0.5f), Parent.Transform, false)];

            public override LightSourceToy[] CreateLights() => [LightSourceToy.Create(new(0.2f, 0f, 0.2f), Parent.Transform, false), LightSourceToy.Create(new(-0.2f, 0f, 0.2f), Parent.Transform, false)];

            public override void Construct()
            {
                CollisionRadius = 0.1f;
                SpinSpeed = 1200f;
                BaseColor = new Color(1f, 0f, 1f, 1f);
                LightColor = new Color(1f, 0f, 1f, 1f);
                LightIntensity = 0.1f;
                UseGravity = false;
                base.Construct();
            }

            public override void Init()
            {
                base.Init();
                initialSpeed = InitialVelocity.magnitude;
                targets = [.. Player.List.Where(p => p != Owner && p.IsAlive && (Owner == null || p.Faction != Owner.Faction))];
            }

            public override void Tick()
            {
                base.Tick();

                if (homing == null)
                {
                    Player targetHoming = null;
                    float dist = float.MaxValue;
                    foreach (Player p in targets)
                    {
                        float distSqr = (p.Position - Rigidbody.position).sqrMagnitude;

                        if (distSqr > homingRangeSqr)
                            continue;

                        if (dist > distSqr)
                        {
                            targetHoming = p;
                            dist = distSqr;
                        }
                    }
                    homing = targetHoming;
                }
                else
                {
                    Vector3 dir = (homing.Position - Rigidbody.position).normalized;
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    Rigidbody.MoveRotation(Quaternion.RotateTowards(Rigidbody.rotation, lookRot, 180f * Time.fixedDeltaTime));
                    Rigidbody.linearVelocity = Rigidbody.transform.forward * initialSpeed;

                    if (!homing.IsAlive)
                    {
                        targets.Remove(homing);
                        homing = null;
                    }
                }
            }

            public override void Hit(Collision col, ReferenceHub player)
            {
                if (player != null)
                {
                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, 20f * (player.IsSCP() ? 3f : 1f), 100, ExplosionType.Disruptor));

                    if (player.roleManager.CurrentRole is IFpcRole role)
                    {
                        role.FpcModule.Motor.JumpController.ForceJump(1f);
                    }

                    Owner?.SendHitMarker();
                }

                LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                toy.Color = BaseColor;
                toy.Intensity = 10f;
                LightExplosion.Create(toy, 40f);
                Destroy();
            }
        }
    }
}
