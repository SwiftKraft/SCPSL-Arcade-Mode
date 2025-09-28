using Footprinting;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using RelativePositioning;
using SwiftArcadeMode.Utils.Projectiles;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Wizard
{
    public class MagicMissile : SpellBase
    {
        public override string Name => "Magic Missile";

        public override Color BaseColor => Color.magenta;

        public override Color AccentColor => Color.white;

        public override int RankIndex => 2;

        public override float CastTime => 0.5f;

        CoroutineHandle coroutine;

        public override void Cast()
        {
            new Projectile(Wizard.Player.Camera.position, Wizard.Player.Camera.rotation, Wizard.Player.Camera.forward * 8f, 20f, Wizard.Player);

            coroutine = Timing.CallPeriodically(1.6f, 0.2f, () =>
            {
                if (!Wizard.Player.IsAlive)
                {
                    Timing.KillCoroutines(coroutine);
                    return;
                }

                new Projectile(Wizard.Player.Camera.position + Wizard.Player.Camera.forward * 0.4f, Wizard.Player.Camera.rotation, Wizard.Player.Camera.forward * 8f, 10f, Wizard.Player);
            });
        }

        public class Projectile(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : ProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            PrimitiveObjectToy ball;
            LightSourceToy light;
            LightSourceToy light2;

            public override void Construct()
            {
                CollisionRadius = 0.1f;

                ball = PrimitiveObjectToy.Create(default, Quaternion.identity, new(0.2f, 0.2f, 0.5f), Parent.Transform, false);
                light = LightSourceToy.Create(new(0.2f, 0f, 0.2f), Parent.Transform, false);
                light2 = LightSourceToy.Create(new(-0.2f, 0f, 0.2f), Parent.Transform, false);

                light.Color = new Color(1f, 0f, 1f, 1f);
                light.Intensity = 0.1f;

                light2.Color = new Color(1f, 0f, 1f, 1f);
                light2.Intensity = 0.1f;

                ball.Type = PrimitiveType.Sphere;
                ball.Color = new Color(1f, 0f, 1f, 1f);
                ball.Flags = AdminToys.PrimitiveFlags.Visible;
                Rigidbody.useGravity = false;
            }

            public override void Init()
            {
                base.Init();
                light.Spawn();
                light2.Spawn();
                ball.Spawn();
            }

            public override void Tick()
            {
                base.Tick();
                Rigidbody.transform.Rotate(Vector3.forward * (Time.fixedDeltaTime * 1200f), Space.Self);
            }

            public override void Hit(Collision col, ReferenceHub player)
            {
                if (player != null)
                {
                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, 25f, 100, ExplosionType.Disruptor));

                    if (player.roleManager.CurrentRole is IFpcRole role)
                    {
                        role.FpcModule.ServerOverridePosition(role.FpcModule.Position + Rigidbody.linearVelocity.normalized * 0.1f);
                        role.FpcModule.Motor.JumpController.ForceJump(1f);
                    }

                    Owner?.SendHitMarker();
                }

                Destroy();
            }

            public override void Destroy()
            {
                base.Destroy();
                if (ball.GameObject != null)
                    ball.Destroy();
                if (light.GameObject != null)
                    light.Destroy();
                if (light2.GameObject != null)
                    light2.Destroy();
            }
        }
    }
}
