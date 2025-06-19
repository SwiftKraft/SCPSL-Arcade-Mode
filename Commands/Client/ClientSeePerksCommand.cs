using CommandSystem;
using Hints;
using LabApi.Features.Wrappers;
using SwiftUHC.Features;
using System;
using System.Text;

namespace SwiftUHC.Commands.Client
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
            StringBuilder hint = new("<align=\"left\">Current Perks: \n");

            foreach (PerkBase perk in PerkManager.Inventories[p].Perks)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(perk.FancyName);
                hint.Append(perk.FancyName);
                hint.AppendLine();
                stringBuilder.Append(" - ");
                stringBuilder.AppendLine(perk.Description);
            }

            hint.Append("</align>");
            p.SendHint(hint.ToString(), [HintEffectPresets.FadeOut()], 10f);
            response = stringBuilder.ToString();
            return true;
        }
    }
}
