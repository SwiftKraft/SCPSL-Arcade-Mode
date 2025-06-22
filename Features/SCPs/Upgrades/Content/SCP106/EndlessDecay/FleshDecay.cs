using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftUHC.Features.SCPs.Upgrades.Content.SCP106.EndlessDecay
{
    public class FleshDecay(UpgradePathPerkBase parent) : UpgradeCooldownTriggerBase<EndlessDecay>(parent)
    {
        public override string Name => "Flesh Decay";

        public override string UpgradeDescription => $"Damages humans around you by {Amount} HP.";

        public virtual float Amount => 2f;
        public virtual float Radius => 5f;

        public override float Cooldown => 1f;

        public override void Effect()
        {
            foreach (Player p in Player.List.Where((p) => (p.Position - Player.Position).sqrMagnitude <= Radius * Radius))
                p.Damage(new ScpDamageHandler(Player.ReferenceHub, Amount, DeathTranslations.PocketDecay));
        }
    }
}
