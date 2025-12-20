using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Deployable;
using SwiftArcadeMode.Utils.Extensions;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class SummonThornShooter : SummonSpell
    {
        public override string Name => "Summon Thorn Shooter";

        public override Color BaseColor => new(0f, 0.6f, 0.1f);

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override int Limit => 2;

        public override DeployableBase Create(Vector3 loc) => new Shooter(Caster.Player.DisplayName + "'s Thorn Shooter", "ThornShooter".ApplySchematicPrefix(), Caster.Player.Role, new Vector3(1f, 0.5f, 1f), loc, Quaternion.identity)
        {
            Owner = Caster.Player
        };

        public class Shooter(string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : TurretSummon(name, schematicName, role, colliderScale, position, rotation)
        {
            public override string TypeName => "Thorn Shooter";
            public override float Health => 200f;
            public override float Range => 10f;
            public override float Delay => 0.25f;

            public override void Attack(Player target)
            {
                Vector3 direction = Quaternion.Euler(Random.insideUnitCircle * 5f) * (target.Camera.position - Dummy.Camera.position).normalized;
                new ThornShot.Projectile(null, Dummy.Camera.position, Quaternion.LookRotation(direction), direction * 40f, 4f, Dummy);
            }
        }
    }
}
