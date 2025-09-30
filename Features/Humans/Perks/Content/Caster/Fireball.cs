using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class Fireball : SpellBase
    {
        public override string Name => "Fireball";

        public override Color BaseColor => Color.red;

        public override Color AccentColor => Color.yellow;

        public override int RankIndex => 1;

        public override float CastTime => 1.3f;

        public override void Cast() => new Projectile(Caster.Player.Camera.position + (Caster.Player.Camera.forward * 0.5f), Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 11f, 15f, Caster.Player);

        public class Projectile(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : Caster.MagicProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            public override LightSourceToy[] CreateLights() => [LightSourceToy.Create(Vector3.down * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.up * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.left * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.forward * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.back * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.right * 0.4f, Parent.Transform, false)];
            public override PrimitiveObjectToy[] CreateBalls() => [PrimitiveObjectToy.Create(default, Quaternion.identity, Vector3.one * 0.5f, Parent.Transform, false)];

            public override void Construct()
            {
                CollisionRadius = 0.25f;
                SpinSpeed = 720f;
                BaseColor = new(1f, 0.5f, 0f);
                LightColor = new(1f, 0.7f, 0f);
                LightIntensity = 5f;
                UseGravity = false;
                base.Construct();
            }

            public void Explode()
            {
                TimedGrenadeProjectile proj = TimedGrenadeProjectile.SpawnActive(Parent.Position, ItemType.GrenadeHE, Owner, 0f);
                if (proj.Base is ExplosionGrenade gr)
                {
                    gr.ScpDamageMultiplier = 2f;
                    gr.MaxRadius = 3f;
                }

                foreach (Player player in Player.List)
                {
                    float distSqr = (player.Position - Rigidbody.position).sqrMagnitude;
                    if (player != null && distSqr <= 25f && player.RoleBase is IFpcRole fpc)
                        fpc.FpcModule.Motor.JumpController.ForceJump(Mathf.Lerp(1f, 10f, Mathf.InverseLerp(25f, 4f, distSqr)));
                }
            }

            public override void EndLife()
            {
                Explode();
                base.EndLife();
            }

            public override void Hit(Collision col, ReferenceHub player)
            {
                Explode();
                Destroy();
            }
        }
    }
}
