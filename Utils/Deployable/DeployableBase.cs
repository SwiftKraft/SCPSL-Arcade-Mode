using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using Mirror;
using NetworkManagerUtils.Dummies;
using PlayerRoles;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace SwiftArcadeMode.Utils.Deployable
{
    public abstract class DeployableBase
    {
        public string Name { get; private set; }

        public SchematicObject Schematic { get; set; }

        public Player Dummy { get; set; }

        public DeployableBase(string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position)
        {
            Dummy = Player.Get(DummyUtils.SpawnDummy(name));
            Dummy.Role = role;
            Dummy.Scale = colliderScale;
            Dummy.Position = position;
            Schematic = ObjectSpawner.SpawnSchematic(schematicName, position);
            Dummy.EnableEffect<Fade>(byte.MaxValue);
            DeployableManager.AllDeployables.Add(this);
        }

        public abstract void Tick();

        public virtual void Destroy()
        {
            DeployableManager.AllDeployables.Remove(this);
            NetworkServer.Destroy(Dummy.GameObject);
            Schematic.Destroy();
        }
    }
}
