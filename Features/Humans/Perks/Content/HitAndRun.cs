﻿using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("HitAndRun", Rarity.Common, conflictPerks: typeof(RaceCar))]
    public class HitAndRun(PerkInventory inv) : PerkKillBase(inv)
    {
        public override string Name => "Hit and Run";

        public override string Description => "When killing a player, gain a speed boost.";

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player)
                return;

            Player.EnableEffect<MovementBoost>(70, 5f, true);
        }
    }
}
