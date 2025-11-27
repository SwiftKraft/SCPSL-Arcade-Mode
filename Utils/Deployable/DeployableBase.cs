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

        public virtual string TypeName => GetType().Name;

        public Vector3 Position
        {
            get => position;
            set
            {
                position = value;
                Dummy.Position = position;
                Schematic.Position = position;
            }
        }
        private Vector3 position;

        public Quaternion Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                Dummy.Rotation = rotation;
                Schematic.Rotation = rotation;
            }
        }
        private Quaternion rotation;

        public SchematicObject Schematic { get; set; }

        public Player Dummy { get; set; }

        public DeployableBase(string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation)
        {
            Name = name;
            Dummy = Player.Get(DummyUtils.SpawnDummy(Name));
            Dummy.CustomInfo = TypeName;
            Dummy.Role = role;
            Dummy.Scale = colliderScale;
            Schematic = ObjectSpawner.SpawnSchematic(schematicName, position, rotation);
            Position = position;
            Rotation = rotation;
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
