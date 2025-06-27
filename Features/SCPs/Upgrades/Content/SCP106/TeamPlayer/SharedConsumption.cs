using LabApi.Features.Wrappers;
using System.Linq;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.TeamPlayer
{
    public class SharedConsumption(UpgradePathPerkBase parent) : LifeConsumption(parent)
    {
        public override string Name => "Shared Consumption";

        public override string Description => base.Description + " Your team receives this as well.";

        public override void Effect()
        {
            foreach (Player p in Player.List.Where((p) => p.IsSCP && p.Role != PlayerRoles.RoleTypeId.Scp0492))
                p.HumeShield += CurrentAmount;
        }
    }
}
