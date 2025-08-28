using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using System.Linq;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Bully", Rarity.Rare)]
    public class Bully(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Bully";

        public override string Description => "Push people when you are near them.";

        public virtual float Strength => 5f;
        public virtual float Distance => 0.4f;

        public override void Tick()
        {
            foreach (Player p in Player.List.Where(p => p != Player && (p.Position - Player.Position).sqrMagnitude <= Distance * Distance))
                if (p.RoleBase is IFpcRole role)
                {
                    Vector3 dirForce = (p.Position - Player.Position).normalized * (Strength * Time.fixedDeltaTime);
                    role.FpcModule.ServerOverridePosition(p.Position + dirForce);
                }

        }
    }
}
