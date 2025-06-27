using CommandSystem;
using Hints;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Commands.Client
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientChooseUpgradeCommand : ICommand
    {
        public string Command => "chooseupgrade";

        public string[] Aliases => ["choose", "c"];

        public string Description => "Chooses an upgrade.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);

            if (arguments.Array.Length < 2 || !int.TryParse(arguments.Array[1], out int num))
            {
                response = "Please input a number!";
                return false;
            }

            if (p.TryGetPerkInventory(out PerkInventory inv) && inv.UpgradeQueue.Upgrades.Count > 0)
            {
                bool success = inv.UpgradeQueue.Choose(num - 1, out string name);

                response = success ? ("Upgrade chosen: " + name + (inv.UpgradeQueue.Upgrades.Count > 0 ? ", " + inv.UpgradeQueue.Upgrades.Count + " more upgrade choices remain." : "")) : "Invalid index.";
                return success;
            }

            response = "No upgrades available.";
            return false;
        }
    }
}
