using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Deployable;
using SwiftArcadeMode.Utils.Extensions;
using SwiftArcadeMode.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class SummonArcaneGolem : SpellBase
    {
        public override string Name => "Summon Arcane Golem";

        public override Color BaseColor => new(0.8f, 0f, 0.8f);

        public override int RankIndex => 2;

        public override float CastTime => 1f;

        Golem currentGolem;

        public override void Cast()
        {
            if (Physics.Raycast(Caster.Player.Camera.position, Vector3.down, out RaycastHit hit, 4f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
            {
                Vector3 loc = hit.point + Vector3.up;
                if (currentGolem == null || currentGolem.Destroyed)
                    currentGolem = new(Caster.Player.DisplayName + "'s Golem", "ArcaneGolem".ApplySchematicPrefix(), Caster.Player.Role, new Vector3(1f, 0.5f, 1f), loc, Quaternion.identity)
                    {
                        Owner = Caster.Player
                    };
                else
                    currentGolem.Position = loc;
            }
        }

        public class Golem(string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : DeployableBase(name, schematicName, role, colliderScale, position, rotation)
        {
            public static readonly LayerMask LOSMask = LayerMask.GetMask("Default", "Door", "Glass");
            public override string TypeName => "Arcane Golem";
            public Player Owner { get; set; }
            public Timer AttackTimer = new();
            public virtual float AttackRange => 15f;

            public override void Initialize()
            {
                base.Initialize();
                Dummy.MaxHealth = 300f;
                Dummy.Health = 300f;
                AttackTimer.Reset(1f);
            }
            public override void Tick()
            {
                base.Tick();
                AttackTimer.Tick(Time.fixedDeltaTime);
                if (AttackTimer.Ended)
                {
                    AttackTimer.Reset(0.5f);
                    IEnumerable<Player> targets = Player.GetAll().Where(p => p.IsAlive && p.Faction != Dummy.Faction && (Position - p.Position).sqrMagnitude <= AttackRange * AttackRange && CheckLOS(p.Camera.position));
                    if (targets.Count() <= 0)
                        return;

                    Player target = null;
                    foreach (Player p in targets)
                    {
                        if (target == null || (p.Position - Position).sqrMagnitude < (target.Position - Position).sqrMagnitude)
                            target = p;
                    }

                    if (target == null)
                        return;
                    Vector3 direction = (target.Position - Dummy.Camera.position).normalized;

                    new MagicMissile.Projectile(null, Dummy.Camera.position, Quaternion.LookRotation(direction), direction * 9f, 5f, Dummy);
                }
            }

            public bool CheckLOS(Vector3 pos) => !Physics.Linecast(Dummy.Camera.position, pos, LOSMask, QueryTriggerInteraction.Ignore);
        }
    }
}
