using CustomPlayerEffects;
using Footprinting;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Wizard
{
    public class IceBolt : SpellBase
    {
        public override string Name => "Ice Bolt";

        public override Color BaseColor => Color.cyan;

        public override Color AccentColor => Color.white;

        public override int RankIndex => 3;

        public override float CastTime => 1f;

        public override void Cast() => new Projectile(Wizard.Player.Camera.position + Wizard.Player.Camera.forward * 0.4f, Wizard.Player.Camera.rotation, Wizard.Player.Camera.forward * 25f, 10f, Wizard.Player);

        public class Projectile(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : ProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            PrimitiveObjectToy ball;
            LightSourceToy light;
            LightSourceToy light2;

            public override void Construct()
            {
                CollisionRadius = 0.1f;
                ball = PrimitiveObjectToy.Create(default, Quaternion.identity, new(0.2f, 0.2f, 0.7f), Parent.Transform, false);
                light = LightSourceToy.Create(new(0.125f, 0f, 0f), Parent.Transform, false);
                light2 = LightSourceToy.Create(new(-0.125f, 0f, 0f), Parent.Transform, false);

                light.Color = new Color(0f, 1f, 1f, 1f);
                light.Intensity = 3f;

                light2.Color = new Color(0f, 1f, 1f, 1f);
                light2.Intensity = 3f;

                ball.Type = PrimitiveType.Sphere;
                ball.Color = new Color(0f, 1f, 1f, 1f);
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
                Rigidbody.transform.Rotate(Vector3.back * (Time.fixedDeltaTime * 700f), Space.Self);
            }

            public override void Hit(Collision col, ReferenceHub player)
            {
                if (player != null)
                {
                    float damage = 40f;

                    if (player.playerEffectsController.TryGetEffect<Ensnared>(out var playerEffect) && playerEffect != null)
                    {
                        if (!playerEffect.IsEnabled)
                            player.playerEffectsController.EnableEffect<Ensnared>(1f);
                        else
                            damage = 60f;
                    }

                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage, 100, ExplosionType.Disruptor));
                    Owner?.SendHitMarker(2f);
                }

                TimedGrenadeProjectile.PlayEffect(Rigidbody.position, ItemType.GrenadeFlash);
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
