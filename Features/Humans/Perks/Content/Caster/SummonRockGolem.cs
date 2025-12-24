using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Deployable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class SummonRockGolem : SummonSpell
    {
        public override string Name => "Rock Golem";

        public override Color BaseColor => new(0.3f, 0.3f, 0.3f);

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override DeployableBase Create(Vector3 loc)
        {
            throw new NotImplementedException();
        }

        public class Golem(string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : TurretSummon(name, schematicName, role, colliderScale, position, rotation)
        {
            public override float Range => 15;

            public override float Delay => 1f;

            public override float Health => 700f;

            public override void Attack(Player target)
            {
                
            }

            public class Projectile(SpellBase spell, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10, Player owner = null) : CasterBase.MagicProjectileBase(spell, initialPosition, initialRotation, initialVelocity, lifetime, owner)
            {
                public override bool UseGravity => true;

                public override float CollisionRadius => 0.3f;

                public override void Hit(Collision col, ReferenceHub hit)
                {
                    
                }
            }
        }
    }
}
