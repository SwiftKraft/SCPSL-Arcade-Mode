using CommandSystem;
using LabApi.Features.Wrappers;
using SwiftUHC.Features;
using SwiftUHC.Features.SCPs.Upgrades;
using System;

namespace SwiftUHC.Commands.RA
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class UpgradeCommand : ICommand
    {
        public string Command => "upgradeperk";

        public string[] Aliases => ["uperk", "upgradep", "up"];

        public string Description => "Upgrades an upgrade path for you or a player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.Effects))
            {
                response = "No permission! ";
                return false;
            }

            Player p = Player.Get(sender);

            if (arguments.Array.Length < 2 || !PerkManager.TryGetPerk(arguments.Array[1].ToLower(), out PerkAttribute t))
            {
                response = "Unknown perk! ";
                return false;
            }

            if (arguments.Array.Length > 2)
                p = int.TryParse(arguments.Array[2], out int id) ? Player.Get(id) : Player.Get(arguments.Array[2]);

            if (!p.TryGetPerkInventory(out PerkInventory inv) || !inv.TryGetPerk(t.Perk, out PerkBase perk) || perk is not UpgradePathPerkBase upg)
            {
                response = "Couldn't find the upgrade path!";
                return false;
            }

            upg.Progress++;

            response = "Upgraded perk: " + t.ID + " for " + p.Nickname;
            return true;
        }
    }
}
