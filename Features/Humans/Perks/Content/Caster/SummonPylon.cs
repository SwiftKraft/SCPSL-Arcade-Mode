using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Deployable;
using SwiftArcadeMode.Utils.Extensions;
using SwiftArcadeMode.Utils.Structures;
using SwiftArcadeMode.Utils.Visuals;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class SummonPylon : SpellBase
    {
        public override string Name => "Summon Pylon";

        public override Color BaseColor => Color.cyan;

        public override int RankIndex => 1;

        public override float CastTime => 1f;

        Pylon currentPylon;

        public override void Cast()
        {
            if (Physics.Raycast(Caster.Player.Camera.position, Vector3.down, out RaycastHit hit, 4f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
            {
                Vector3 loc = hit.point + Vector3.up;
                if (currentPylon == null || currentPylon.Destroyed)
                    currentPylon = new(Caster.Player.DisplayName + "'s Pylon", "Pylon".ApplySchematicPrefix(), Caster.Player.Role, new Vector3(1f, 0.5f, 1f), loc, Quaternion.identity)
                    {
                        Owner = Caster.Player
                    };
                else
                    currentPylon.Position = loc;
            }
        }

        public class Pylon(string name, string schematicName, RoleTypeId role, Vector3 colliderScale, Vector3 position, Quaternion rotation) : DeployableBase(name, schematicName, role, colliderScale, position, rotation)
        {
            public override string TypeName => "Healing Pylon";

            public Player Owner { get; set; }

            public Timer HealDelay = new();

            public override void Initialize()
            {
                base.Initialize();
                Dummy.MaxHealth = 500f;
                Dummy.Health = 500f;
                HealDelay.Reset(2f);
            }

            public override void Tick()
            {
                base.Tick();
                HealDelay.Tick(Time.fixedDeltaTime);
                if (HealDelay.Ended)
                {
                    HealDelay.Reset();
                    SchematicEffect.Create("PylonVfx".ApplySchematicPrefix(), Position, Rotation, Vector3.one, 0.5f);
                    foreach (Player p in Player.List)
                    {
                        if (p == Dummy || p.Faction != Owner.Faction || !p.IsAlive || (p.Position - Dummy.Position).sqrMagnitude > 16f)
                            continue;

                        p.Heal(10f);
                    }
                }
            }

            public override void Destroy()
            {
                TimedGrenadeProjectile proj = TimedGrenadeProjectile.SpawnActive(Position, ItemType.GrenadeHE, null, 0f);
                if (proj.Base is ExplosionGrenade gr)
                {
                    gr.ScpDamageMultiplier = 1f;
                    gr.MaxRadius = 4f;
                }
                base.Destroy();
            }
        }
    }
}
