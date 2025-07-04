﻿using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using System.Linq;

namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    public static class UpgradePathGiver
    {
        public static int SCPTeamExperience
        {
            get => _scpTeamExperience;
            set
            {
                if (_scpTeamExperience == value)
                    return;

                _scpTeamExperience = value;

                while (_scpTeamExperience >= Requirement)
                {
                    _scpTeamExperience -= Requirement;
                    SCPLevel++;
                }
            }
        }
        private static int _scpTeamExperience;

        public static int Requirement => SCPLevel * 4;

        public static int SCPLevel
        {
            get => _scpLevel;
            set
            {
                if (_scpLevel == value)
                    return;

                if (_scpLevel < value)
                    for (int i = 0; i < value - _scpLevel; i++)
                        foreach (Player p in Player.List)
                            if (p.IsSCP && p.TryGetPerkInventory(out PerkInventory inv))
                            {
                                inv.UpgradeQueue.Create(3, UpgradePathManager.RegisteredUpgrades.Where((u) => inv.TryGetPerk(u.Perk.Perk, out PerkBase ba) && ba is UpgradePathPerkBase b && b.Maxed).ToList());
                                p.SendBroadcast("SCP Team Leveled Up! \nCurrent Level: " + value, 5);
                            }

                _scpLevel = value;
            }
        }
        private static int _scpLevel = 1;

        public static void Enable()
        {
            PlayerEvents.Dying += OnPlayerDying;
        }

        public static void Disable()
        {
            PlayerEvents.Dying -= OnPlayerDying;
        }

        private static void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Player.IsHuman && ((ev.Attacker != null && ev.Attacker.IsSCP) || (ev.DamageHandler is UniversalDamageHandler dmg && dmg.TranslationId == DeathTranslations.PocketDecay.Id)))
                SCPTeamExperience++;
        }
    }
}
