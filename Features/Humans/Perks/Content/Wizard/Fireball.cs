using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using SwiftArcadeMode.Utils.Projectiles;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Wizard
{
    public class Fireball : SpellBase
    {
        public override string Name => "Fireball";

        public override Color BaseColor => Color.red;

        public override Color AccentColor => Color.yellow;

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override void Cast() => new Projectile(Wizard.Player.Camera.position + (Wizard.Player.Camera.forward * 0.5f), Wizard.Player.Camera.rotation, Wizard.Player.Camera.forward * 10f, 15f, Wizard.Player);

        public class Projectile(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : ProjectileBase(initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            PrimitiveObjectToy ball;
            LightSourceToy[] lights;

            public override void Construct()
            {
                CollisionRadius = 0.25f;
                ball = PrimitiveObjectToy.Create(default, Quaternion.identity, Vector3.one * 0.5f, Parent.Transform, false);
                lights = [LightSourceToy.Create(Vector3.down * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.up * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.left * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.forward * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.back * 0.4f, Parent.Transform, false), LightSourceToy.Create(Vector3.right * 0.4f, Parent.Transform, false)];

                ball.Type = PrimitiveType.Sphere;
                ball.Color = new Color(1f, 0.5f, 0f, 1f);
                ball.Flags = AdminToys.PrimitiveFlags.Visible;
                Rigidbody.useGravity = false;
            }

            public override void Init()
            {
                base.Init();
                ball.Spawn();

                foreach (var light in lights)
                {
                    light.Color = new Color(1f, 0.5f, 0f, 1f);
                    light.Intensity = 5f;
                    light.Spawn();
                }
            }

            public override void Tick()
            {
                base.Tick();
                Rigidbody.transform.Rotate(Vector3.forward * (Time.fixedDeltaTime * 720f), Space.Self);
            }

            public override void Hit(Collision col, ReferenceHub player)
            {
                TimedGrenadeProjectile proj = TimedGrenadeProjectile.SpawnActive(Parent.Position, ItemType.GrenadeHE, Owner, 0f);
                if (proj.Base is ExplosionGrenade gr)
                    gr.ScpDamageMultiplier = 2f;
                if (player != null && player.roleManager.CurrentRole is IFpcRole fpc)
                    fpc.FpcModule.Motor.JumpController.ForceJump(10f);
                Destroy();
            }

            public override void Destroy()
            {
                base.Destroy();
                if (ball.GameObject != null)
                    ball.Destroy();
                foreach (var light in lights)
                {
                    if (light.GameObject != null)
                        light.Destroy();
                }
            }
        }
    }
}
