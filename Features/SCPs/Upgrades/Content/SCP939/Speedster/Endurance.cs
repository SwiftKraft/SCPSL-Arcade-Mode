using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using PlayerStatsSystem;
using SwiftUHC.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftUHC.Features.SCPs.Upgrades.Content.SCP939.Speedster
{
    public class Endurance(UpgradePathPerkBase parent) : UpgradeKillBase<Speedster>(parent)
    {
        public override string Name => "Endurance";

        public override string Description => "Replenish your stamina on kill.";

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player)
                return;

            Player.StaminaRemaining = Player.ReferenceHub.playerStats.GetModule<StaminaStat>().MaxValue;
        }
    }
}
