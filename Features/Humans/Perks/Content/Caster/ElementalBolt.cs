using CustomPlayerEffects;
using Footprinting;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class ElementalBolt : SpellBase
    {
        public override string Name => "Elemental Bolt";

        public override Color BaseColor => new(0.7f, 0.3f, 0f);

        public override int RankIndex => 1;

        public override float CastTime => 1.1f;

        public override void Cast() => new Projectile(Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 25f, 10f, Caster.Player);

        public class Projectile(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : CasterBase.MagicProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            public override PrimitiveObjectToy[] CreateBalls() => [PrimitiveObjectToy.Create(Vector3.up * 0.1f, Quaternion.identity, new(0.05f, 0.05f, 0.4f), Parent.Transform, false), PrimitiveObjectToy.Create(Vector3.down * 0.1f, Quaternion.identity, new(0.05f, 0.05f, 0.4f), Parent.Transform, false), PrimitiveObjectToy.Create(Vector3.left * 0.1f, Quaternion.identity, new(0.05f, 0.05f, 0.4f), Parent.Transform, false), PrimitiveObjectToy.Create(Vector3.right * 0.1f, Quaternion.identity, new(0.05f, 0.05f, 0.4f), Parent.Transform, false)];

            public override LightSourceToy[] CreateLights() => [LightSourceToy.Create(Parent.Transform, false), LightSourceToy.Create(new(0.1f, 0.1f, 0f), Parent.Transform, false), LightSourceToy.Create(new(-0.1f, 0.1f, 0f), Parent.Transform, false), LightSourceToy.Create(new(0.1f, -0.1f, 0f), Parent.Transform, false), LightSourceToy.Create(new(-0.1f, -0.1f, 0f), Parent.Transform, false)];

            public override void Construct()
            {
                CollisionRadius = 0.2f;
                SpinSpeed = -300f;
                BaseColor = new(0.7f, 0.3f, 0f);
                LightColor = new(1f, 1f, 1f);
                LightIntensity = 10f;
                UseGravity = false;
                base.Construct();
            }

            public override void Hit(Collision col, ReferenceHub player)
            {
                if (player != null)
                {
                    float damage = 100f;

                    if (player.playerEffectsController.TryGetEffect<Burned>(out var playerEffect) && playerEffect != null)
                    {
                        if (!playerEffect.IsEnabled)
                            player.playerEffectsController.EnableEffect<Burned>(5f, true);
                        else
                            damage = 150f;
                    }

                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage * (player.IsSCP() ? 3f : 1f), 100, ExplosionType.Disruptor));
                    Owner?.SendHitMarker(2f);
                }

                LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                toy.Color = BaseColor;
                toy.Intensity = 15f;

                LightExplosion.Create(toy, 30f);
                Destroy();
            }
        }
    }
}
