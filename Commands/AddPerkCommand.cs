using CommandSystem;
using LabApi.Features.Wrappers;
using SwiftUHC.Features.Humans.Perks;
using System;

namespace SwiftUHC.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AddPerkCommand : ICommand
    {
        public string Command => "addperk";

        public string[] Aliases => ["aperk", "addp", "ap"];

        public string Description => "Adds a perk to you or a player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);

            if (arguments.Array.Length < 2 || !PerkManager.TryGetPerk(arguments.Array[1].ToLower(), out PerkAttribute t))
            {
                response = "Unknown perk! ";
                return false;
            }

            if (arguments.Array.Length > 2)
                p = int.TryParse(arguments.Array[2], out int id) ? Player.Get(id) : Player.Get(arguments.Array[2]);

            PerkManager.GivePerk(p, t);

            response = "Added perk: " + t.ID + " to " + p.Nickname;
            return true;
        }
    }
}
