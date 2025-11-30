using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Deployable;
using SwiftArcadeMode.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class SummonPylon : SpellBase
    {
        public override string Name => "Summon Pylon";

        public override Color BaseColor => Color.cyan;

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        public override void Cast()
        {
            new Pylon("Pylon", "Pylon".ApplySchematicPrefix(), Caster.Player.Role, new Vector3(1f, 0.5f, 1f), Caster.Player.Position, Quaternion.identity).Owner = Caster.Player;
        }

        public class Pylon(string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : DeployableBase(name, schematicName, role, colliderScale, position, rotation)
        {
            public Player Owner { get; set; }

            public override void Initialize()
            {
                base.Initialize();
                Dummy.MaxHealth = 500f;
                Dummy.Health = 500f;
            }

            public override void Tick()
            {
                
            }
        }
    }
}
