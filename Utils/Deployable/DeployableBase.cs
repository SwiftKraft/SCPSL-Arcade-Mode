using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using NetworkManagerUtils.Dummies;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using System.Linq;
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
            Timing.CallDelayed(Time.deltaTime, () =>
            {
                Dummy.SetRole(role, RoleChangeReason.None, RoleSpawnFlags.None);
                Dummy.CustomInfo = TypeName;
                Dummy.Scale = colliderScale;
                Dummy.EnableEffect<Fade>(byte.MaxValue);
                Dummy.ReferenceHub.playerStats.OnThisPlayerDied += OnDummyDied;
                Position = position;
                Rotation = rotation;
                //if (Dummy.RoleBase is IFpcRole r)
                //{
                //    HitboxIdentity identity = r.FpcModule.CharacterModelInstance.Hitboxes.FirstOrDefault(h => h.HitboxType == HitboxType.Body);
                //    if (identity != null)
                //    {
                //        Collider col = identity.TargetColliders.FirstOrDefault();
                //        col.transform.localScale = Vector3.one * 3f;
                //    }
                //}
                Initialize();
            });
            Schematic = ObjectSpawner.SpawnSchematic(schematicName, position, rotation);
            DeployableManager.AllDeployables.Add(this);
        }

        private void OnDummyDied(PlayerStatsSystem.DamageHandlerBase obj)
        {
            Dummy.ReferenceHub.playerStats.OnThisPlayerDied -= OnDummyDied;
            Destroy();
        }

        public virtual void Initialize() { }

        public abstract void Tick();

        public virtual void Destroy()
        {
            DeployableManager.AllDeployables.Remove(this);
            NetworkServer.Destroy(Dummy.GameObject);
            Schematic.Destroy();
        }
    }
}
