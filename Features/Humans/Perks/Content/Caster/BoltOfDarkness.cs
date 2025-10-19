using CustomPlayerEffects;
using Footprinting;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Projectiles;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class BoltOfDarkness : SpellBase
    {
        public override string Name => "Bolt of Darkness";

        public override Color BaseColor => Color.black;

        public override int RankIndex => 1;

        public override float CastTime => 0.5f;

        public override void Cast() => new Projectile(Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 35f, 10f, Caster.Player);

        public class Projectile(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : CasterBase.MagicProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            public override PrimitiveObjectToy[] CreateBalls() => [PrimitiveObjectToy.Create(default, Quaternion.identity, new(0.2f, 0.2f, 0.7f), Parent.Transform, false)];

            public override LightSourceToy[] CreateLights() => [LightSourceToy.Create(new(0.125f, 0f, 0f), Parent.Transform, false), LightSourceToy.Create(new(-0.125f, 0f, 0f), Parent.Transform, false)];

            public override void Construct()
            {
                CollisionRadius = 0.1f;
                SpinSpeed = 400f;
                BaseColor = new(0f, 0f, 0f, 1f);
                LightColor = new(1f, 1f, 1f, 1f);
                LightIntensity = 0.1f;
                UseGravity = false;
                base.Construct();
            }

            public override void Hit(Collision col, ReferenceHub player)
            {
                if (player != null)
                {
                    float damage = 75f;
                    player.playerEffectsController.EnableEffect<Sinkhole>(5f);

                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage * (player.IsSCP() ? 5f : 1f), 100, ExplosionType.Disruptor));
                    Owner?.SendHitMarker(2f);
                }

                LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                toy.Color = LightColor;
                toy.Intensity = 1f;
                LightExplosion.Create(toy, 15f);

                Destroy();
            }
        }
    }
}
