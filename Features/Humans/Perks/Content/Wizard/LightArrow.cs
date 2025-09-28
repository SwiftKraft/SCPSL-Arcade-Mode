using Footprinting;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Wizard
{
    public class LightArrow : SpellBase
    {
        public override string Name => "Light Arrow";

        public override Color BaseColor => Color.white;

        public override Color AccentColor => Color.yellow;

        public override int RankIndex => 3;

        public override float CastTime => 0.5f;

        public override void Cast() => new Projectile(Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 30f, 6f, Caster.Player);

        public class Projectile(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 3f, Player owner = null) : Caster.MagicProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            const int maxBounces = 2;
            int currentBounces = 0;
            float initialSpeed;
            float currentDamage = 60f;
            float currentScpMultiplier = 3f;
            Vector3 vel;

            public override PrimitiveObjectToy[] CreateBalls() => [PrimitiveObjectToy.Create(default, Quaternion.identity, new(0.05f, 0.05f, 0.4f), Parent.Transform, false)];
            public override LightSourceToy[] CreateLights() => [LightSourceToy.Create(Vector3.left * 0.1f, Parent.Transform, false), LightSourceToy.Create(Vector3.forward * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.back * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.right * 0.1f, Parent.Transform, false)];

            public override void Init()
            {
                initialSpeed = InitialVelocity.magnitude;
                vel = InitialVelocity;
                base.Init();
            }

            public override void Construct()
            {
                CollisionRadius = 0.08f;
                SpinSpeed = 1500f;
                BaseColor = new(1f, 1f, 1f, 1f);
                LightColor = new(1f, 1f, 1f, 1f);
                LightIntensity = 3f;
                UseGravity = false;
                base.Construct();
            }

            public override void Tick()
            {
                base.Tick();
                Rigidbody.linearVelocity = vel;
            }

            public override void Hit(Collision col, ReferenceHub hit)
            {
                if (hit == null && currentBounces < maxBounces)
                {
                    Vector3 normal = col.GetContact(0).normal.normalized;
                    Vector3 direction = Vector3.Reflect(vel.normalized, -normal);

                    if (FindTargetInDirection(direction, 25f, 15f, out Vector3 dir))
                        direction = dir;

                    Rigidbody.angularVelocity = Vector3.zero;
                    Rigidbody.transform.forward = direction.normalized;
                    vel = direction.normalized * initialSpeed;

                    currentDamage += 25f;
                    currentScpMultiplier += 0.5f;
                    currentBounces++;
                }
                else
                {
                    if (hit != null)
                    {
                        hit.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, currentDamage * (hit.IsSCP() ? currentScpMultiplier : 1f), 100, ExplosionType.Disruptor));

                        if (hit.roleManager.CurrentRole is IFpcRole role)
                        {
                            role.FpcModule.ServerOverridePosition(role.FpcModule.Position + Rigidbody.linearVelocity.normalized * 0.1f);
                            role.FpcModule.Motor.JumpController.ForceJump(1f);
                        }

                        Owner?.SendHitMarker(2f);
                    }

                    TimedGrenadeProjectile.PlayEffect(Rigidbody.position, ItemType.GrenadeFlash);
                    Destroy();
                }
            }

            private bool FindTargetInDirection(Vector3 dir, float maxDistance, float maxAngle, out Vector3 dirToTarget)
            {
                Player best = null;
                float bestDot = 0f;
                dirToTarget = default;

                foreach (Player player in Player.List)
                {
                    if (player == Owner || !player.IsAlive || (Owner != null && player.Faction == Owner.Faction)) 
                        continue;

                    Vector3 toTarget = player.Position - Rigidbody.position;
                    float dist = toTarget.magnitude;

                    if (dist > maxDistance) continue;

                    Vector3 _dirToTarget = toTarget.normalized;
                    float dot = Vector3.Dot(dir.normalized, _dirToTarget);

                    if (dot > Mathf.Cos(maxAngle * Mathf.Deg2Rad) && dot > bestDot)
                    {
                        bestDot = dot;
                        best = player;
                        dirToTarget = _dirToTarget;
                    }
                }

                return best != null;
            }
        }
    }
}
