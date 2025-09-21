using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp939Events;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles.PlayableScps.Scp939;
using SwiftArcadeMode.Utils.Structures;
using System.Collections.Generic;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Diseased
{
    public class Bleeding(UpgradePathPerkBase parent) : UpgradeBase<Diseased>(parent)
    {
        public readonly Dictionary<Player, Timer> Players = [];

        public override string Name => "Bleeding";
        public override string Description => $"Hitting a human will cause {TotalDamage} damage over {Duration}s.";

        public float TotalDamage => 15f;
        public float Duration => 5f;

        public override void Init()
        {
            base.Init();
            Scp939Events.Attacking += Attacking;
        }

        public override void Tick()
        {
            base.Tick();
            HashSet<Player> removal = [];

            foreach (Player player in Players.Keys)
            {
                if (!player.IsAlive)
                {
                    removal.Add(player);
                    continue;
                }

                player.Damage(TotalDamage / Duration * Time.fixedDeltaTime, Player, armorPenetration: 100);
                Players[player].Tick(Time.fixedDeltaTime);

                if (Players[player].Ended)
                    removal.Add(player);
            }

            foreach (Player pl in removal)
                Players.Remove(pl);
        }

        public override void Remove()
        {
            base.Remove();
            Scp939Events.Attacking -= Attacking;
        }

        private void Attacking(Scp939AttackingEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            if (!Players.ContainsKey(ev.Target))
                Players.Add(ev.Target, new(Duration));
            else
                Players[ev.Target].Reset();
        }
    }
}
