﻿using SwiftArcadeMode.Utils.Extensions;
using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP3114
{
    public class RandomPerk(UpgradePathPerkBase parent) : UpgradeBase<RandomPerks>(parent)
    {
        public override string Name => $"Random Perk ({Rarity})";

        public override string Description => "Gives you a random perk.";

        public Rarity Rarity { get; set; } = Enum.GetValues(typeof(Rarity)).ToArray<Rarity>().GetRandom();

        public override void Init()
        {
            base.Init();
            PerkAttribute att = PerkManager.GetRandomPerk(p => p.Rarity == Rarity);
            if (!Player.HasPerk(att.Perk))
                Player.GivePerk(att);
        }
    }
}
