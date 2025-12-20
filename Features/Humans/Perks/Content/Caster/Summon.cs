using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Deployable;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public abstract class Summon(string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : DeployableBase(name, schematicName, role, colliderScale, position, rotation)
    {
        public Player Owner { get; set; }

        public abstract float Health { get; }

        public override void Initialize()
        {
            base.Initialize();
            Dummy.MaxHealth = Health;
            Dummy.Health = Health;
        }
    }
}
