using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Baller", Rarity.Legendary)]
    public class Baller(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Baller";
        public override string Description => "Baller.";

        PrimitiveObjectToy ball;
        Rigidbody rb;

        public override void Init()
        {
            base.Init();
            ball = PrimitiveObjectToy.Create(Player.Position, Quaternion.identity, Vector3.one * 0.3f, networkSpawn: false);
            ball.Type = PrimitiveType.Sphere;
            ball.Color = Color.red;
            rb = ball.Base.gameObject.AddComponent<Rigidbody>();
            rb.mass = 0.2f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            ball.Spawn();
        }

        public override void Tick()
        {
            base.Tick();
            foreach (Player p in Player.List)
            {
                if (!p.IsAlive || p.RoleBase is not IFpcRole role)
                    return;

                Vector3 feetPos = p.Position + Vector3.down;
                if ((feetPos - rb.position).sqrMagnitude < 0.16f)
                    rb.AddForce((rb.position - feetPos).normalized * role.FpcModule.MaxMovementSpeed, ForceMode.Impulse);
            }
        }

        public override void Remove()
        {
            base.Remove();
            ball.Destroy();
        }
    }
}
