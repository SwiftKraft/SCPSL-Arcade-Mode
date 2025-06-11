using CommandSystem;
using LabApi.Features.Wrappers;
using SwiftUHC.Features.Humans.Perks;
using System;
using System.Text;

namespace SwiftUHC.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientSeePerksCommand : ICommand
    {
        public string Command => "seeperks";

        public string[] Aliases => ["sperk", "sp"];

        public string Description => "Shows you all the perks that you have.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);

            if (!PerkManager.Inventories.ContainsKey(p) || PerkManager.Inventories[p].Perks.Count <= 0)
            {
                response = "\n\nYou have no perks.";
                return true;
            }

            StringBuilder stringBuilder = new("\n\n");

            foreach (PerkBase perk in PerkManager.Inventories[p].Perks)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(perk.FancyName);
                stringBuilder.Append(" - ");
                stringBuilder.AppendLine(perk.Description);
            }

            response = stringBuilder.ToString();
            return true;
        }
    }
}
