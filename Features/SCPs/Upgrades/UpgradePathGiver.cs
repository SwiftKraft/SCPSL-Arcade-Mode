using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System.Linq;

namespace SwiftUHC.Features.SCPs.Upgrades
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
                    Logger.Info("SCPs leveled up!");
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
                        {
                            if (p.IsSCP && p.TryGetPerkInventory(out PerkInventory inv))
                            {
                                inv.UpgradeQueue.Create(3);
                                Logger.Info("Created upgrade choice for " + p);
                            }
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
            if (ev.Player.IsHuman && ev.Attacker.IsSCP)
            {
                SCPTeamExperience++;
                Logger.Info("Killed human.");
            }
        }
    }
}
