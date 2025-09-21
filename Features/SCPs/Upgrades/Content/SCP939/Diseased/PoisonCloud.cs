using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Diseased
{
    public class PoisonCloud(UpgradePathPerkBase parent) : UpgradeBase<Diseased>(parent)
    {
        public override string Name => "Poison Cloud";

        public override string Description => $"Damage everyone that has the amnesia effect by {Damage} HP/s.";

        public float Damage => 3f;

        public override void Tick()
        {
            base.Tick();
            foreach (Player p in Player.List)
                if (p.HasEffect<AmnesiaVision>())
                    p.Damage(Damage * Time.fixedDeltaTime, Player, armorPenetration: 100);
        }
    }
}
