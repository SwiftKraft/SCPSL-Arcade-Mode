using LabApi.Features.Wrappers;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Diseased
{
    public class Bleeding(UpgradePathPerkBase parent) : UpgradeBase<Diseased>(parent)
    {
        public readonly Dictionary<Player, float> Players = [];

        public override string Name => "Bleeding";

        public override string Description => $"Hitting a human will cause {TotalDamage} damage over {Duration}s.";

        public float TotalDamage => 15f;
        public float Duration => 5f;

        public override void Tick()
        {
            base.Tick();
            HashSet<Player> removal = [];
            foreach (Player player in Players.Keys)
            {
                player.Damage(TotalDamage / Duration * Time.fixedDeltaTime, Player, armorPenetration: 100);
                Players[player] -= Time.fixedDeltaTime;
                if (Players[player] <= 0f)
                    removal.Add(player);
            }

            foreach (Player pl in removal)
                Players.Remove(pl);
        }
    }
}
