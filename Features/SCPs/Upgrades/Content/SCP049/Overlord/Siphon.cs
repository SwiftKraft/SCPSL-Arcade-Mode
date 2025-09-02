using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using System.Linq;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.Overlord
{
    public class Siphon(UpgradePathPerkBase parent) : UpgradeBase<Overlord>(parent)
    {
        public override string Name => "Siphon";

        public override string Description => $"Siphons {Amount} HP from nearby zombies when activating <i>The Doctor's Call</i>.";

        public float Amount => 100f;
        public float Range => 15f;

        public override void Init()
        {
            base.Init();
            Scp049Events.UsedDoctorsCall += OnUsedDoctorsCall;
        }

        public override void Remove()
        {
            base.Remove();
            Scp049Events.UsedDoctorsCall -= OnUsedDoctorsCall;
        }

        private void OnUsedDoctorsCall(LabApi.Events.Arguments.Scp049Events.Scp049UsedDoctorsCallEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            foreach (Player p in Player.List.Where(p => p.IsSCP && p.Role == PlayerRoles.RoleTypeId.Scp0492 && (p.Position - Player.Position).sqrMagnitude <= Range * Range))
            {
                if (p.Health <= 1f)
                    continue;

                bool canSupply = p.Health <= Amount;
                Player.Heal(canSupply ? p.Health - 1f : Amount);
                p.Health = canSupply ? 1f : p.Health - Amount;
            }
        }
    }
}
