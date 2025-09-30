using CustomPlayerEffects;
using Footprinting;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
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
    public class IceBolt : SpellBase
    {
        public override string Name => "Ice Bolt";

        public override Color BaseColor => Color.cyan;

        public override Color AccentColor => Color.white;

        public override int RankIndex => 3;

        public override float CastTime => 1f;

        public override void Cast() => new Projectile(Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 25f, 10f, Caster.Player);

        public class Projectile(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : CasterBase.MagicProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            public override PrimitiveObjectToy[] CreateBalls() => [PrimitiveObjectToy.Create(default, Quaternion.identity, new(0.2f, 0.2f, 0.7f), Parent.Transform, false)];

            public override LightSourceToy[] CreateLights() => [LightSourceToy.Create(new(0.125f, 0f, 0f), Parent.Transform, false), LightSourceToy.Create(new(-0.125f, 0f, 0f), Parent.Transform, false)];

            public override void Construct()
            {
                CollisionRadius = 0.1f;
                SpinSpeed = -400f;
                BaseColor = new(0f, 1f, 1f, 1f);
                LightColor = new(0f, 1f, 1f, 1f);
                LightIntensity = 3f;
                UseGravity = false;
                base.Construct();
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

                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage * (player.IsSCP() ? 3f : 1f), 100, ExplosionType.Disruptor));
                    Owner?.SendHitMarker(2f);
                    TimedGrenadeProjectile.PlayEffect(player.transform.position - Vector3.up, ItemType.SCP2176);
                }
                else
                    TimedGrenadeProjectile.PlayEffect(Rigidbody.position, ItemType.SCP2176);

                Destroy();
            }
        }
    }
}
