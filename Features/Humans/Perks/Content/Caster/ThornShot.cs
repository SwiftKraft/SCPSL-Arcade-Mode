﻿using Footprinting;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class ThornShot : SpellBase
    {
        public override string Name => "Thorn Shot";

        public override Color BaseColor => Color.green;

        public override int RankIndex => 2;

        public override float CastTime => 1.3f;

        const float spreadAngle = 65f;
        const int projectileCount = 10;

        public override void Cast()
        {
            float angleStep = spreadAngle / (projectileCount - 1);
            float startAngle = -spreadAngle / 2f;

            List<SphereCollider> colliders = [];
            for (int i = 0; i < projectileCount; i++)
            {
                float currentAngle = startAngle + (angleStep * i);
                Quaternion rotation = Quaternion.Euler(0f, currentAngle, 0f);
                Vector3 direction = rotation * Caster.Player.Camera.forward;

                colliders.Add(new Projectile(this, Caster.Player.Camera.position + direction * 0.25f, Quaternion.LookRotation(direction), direction * 40f, 15f, Caster.Player).Collider);
            }

            for (int i = 0; i < colliders.Count; i++)
                for (int j = 0; j < colliders.Count; j++)
                    Physics.IgnoreCollision(colliders[i], colliders[j], true);

            PlaySound("cast");
        }

        public class Projectile(SpellBase spell, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : CasterBase.MagicProjectileBase(spell, initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            public override string SchematicName => "ThornShot";

            public bool Sticked { get; private set; }

            public override bool UseGravity => false;

            public override float CollisionRadius => 0.01f;

            public override void Tick()
            {
                if (!Sticked)
                    base.Tick();
                else
                {
                    Lifetime.Tick(Time.fixedDeltaTime);
                    if (Lifetime.Ended)
                        EndLife();
                }
            }

            public override void Hit(Collision col, ReferenceHub hit)
            {
                if (hit != null)
                {
                    hit.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, 25f * (hit.IsSCP() ? 3f : 1f), 60, ExplosionType.Disruptor));

                    if (hit.roleManager.CurrentRole is IFpcRole role)
                    {
                        role.FpcModule.Motor.JumpController.ForceJump(1f);
                    }

                    Owner?.SendHitMarker();
                    Destroy();
                }
                else
                {
                    Lifetime.Reset();
                    Sticked = true;
                    Rigidbody.isKinematic = true;
                    Rigidbody.detectCollisions = false;
                }
            }
        }
    }
}
