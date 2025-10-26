﻿using Footprinting;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Projectiles;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class OrbOfNature : SpellBase
    {
        public override string Name => "Orb of Nature";

        public override Color BaseColor => Color.yellow;

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override void Cast()
        {
            new Projectile(this, Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 25f, 10f, Caster.Player);
            PlaySound("cast");
        }


        public class Projectile(SpellBase spell, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : CasterBase.MagicProjectileBase(spell, initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            public override string SchematicName => "OrbOfNature";

            public override bool UseGravity => false;

            public override float CollisionRadius => 0.1f;

            public override void Hit(Collision col, ReferenceHub player)
            {
                if (player != null)
                {
                    float damage = 85f;
                    if (player.GetFaction() != Owner.Faction)
                        player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, damage * (player.IsSCP() ? 3f : 1f), 50, ExplosionType.Disruptor));
                    else
                        player.playerStats.GetModule<HealthStat>().ServerHeal(damage / 2f);
                    Owner?.SendHitMarker(1.5f);
                }

                LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                toy.Intensity = 100f;
                toy.Color = Color.green;
                toy.ShadowType = LightShadows.Hard;
                toy.Range = 30f;
                LightExplosion.Create(toy, 100f);
                Destroy();
            }
        }
    }
}
