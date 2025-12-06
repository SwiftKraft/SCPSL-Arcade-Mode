using CustomPlayerEffects;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
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
            get => Dummy.Position;
            set
            {
                if (Dummy != null)
                    Dummy.Position = value;
                if (Schematic != null)
                    Schematic.Position = Dummy.Position;
            }
        }

        public Quaternion Rotation
        {
            get => Dummy.Rotation;
            set
            {
                if (Dummy != null)
                    Dummy.Rotation = value;
                if (Schematic != null)
                    Schematic.Rotation = Dummy.Rotation;
            }
        }

        public bool Destroyed { get; private set; } = false;

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
                Dummy.EnableEffect<Flashed>();
                Dummy.ReferenceHub.playerStats.OnThisPlayerDied += OnDummyDied;
                Dummy.ReferenceHub.serverRoles.TryHideTag();
                Position = position;
                Rotation = rotation;
                Initialize();
            });
            Schematic = ObjectSpawner.SpawnSchematic(schematicName, position, rotation);
            DeployableManager.AllDeployables.Add(this);
            Scp096Events.AddingTarget += On096AddingTarget;
        }

        private void On096AddingTarget(LabApi.Events.Arguments.Scp096Events.Scp096AddingTargetEventArgs ev)
        {
            if (ev.Target == Dummy)
                ev.IsAllowed = false;
        }

        private void OnDummyDied(PlayerStatsSystem.DamageHandlerBase obj)
        {
            Dummy.ReferenceHub.playerStats.OnThisPlayerDied -= OnDummyDied;
            Destroy();
        }

        public virtual void Initialize() { }

        public virtual void Tick() => Schematic?.transform.SetPositionAndRotation(Dummy.Position, Dummy.Rotation);

        public virtual void Destroy()
        {
            DeployableManager.AllDeployables.Remove(this);
            NetworkServer.Destroy(Dummy.GameObject);
            Schematic.Destroy();
            Scp096Events.AddingTarget -= On096AddingTarget;
            Destroyed = true;
        }
    }
}
