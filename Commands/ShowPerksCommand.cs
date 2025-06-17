using CommandSystem;
using SwiftUHC.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwiftUHC.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ShowPerksCommand : ICommand
    {
        public string Command => "showperks";

        public string[] Aliases => ["sperks", "showp", "sp"];

        public string Description => "Adds a perk to you or a player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder builder = new("All available perks: \n");

            foreach (KeyValuePair<string, PerkAttribute> att in PerkManager.RegisteredPerks)
            {
                builder.Append("  ");
                builder.Append(att.Key);
                builder.Append(" - ");
                builder.Append(att.Value.Profile.FancyName);
                builder.Append("\n");
            }

            response = builder.ToString();

            return true;
        }
    }
}
