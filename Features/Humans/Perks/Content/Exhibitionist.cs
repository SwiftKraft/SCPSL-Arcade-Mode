﻿using LabApi.Features.Wrappers;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Exhib", Rarity.Secret)]
    public class Exhibitionist(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public override string Name => "Exhibitionist";

        public override string PerkDescription => "Flash enemies around you. ";

        public override float Cooldown => 20f;

        public override void Effect() => TimedGrenadeProjectile.SpawnActive(Player.Position, ItemType.GrenadeFlash, Player, 0.1f);
    }
}
