﻿using CommandSystem;
using Hints;
using LabApi.Features.Wrappers;
using SwiftUHC.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftUHC.Commands.Client
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientSeeUpgradesCommand : ICommand
    {
        public string Command => "seeupgrades";

        public string[] Aliases => ["sup", "su"];

        public string Description => "Shows you all the upgrade options that you have.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);

            if (p.TryGetPerkInventory(out PerkInventory inv))
            {
                response = inv.UpgradeQueue.Peek(out string brief);
                p.SendHint("<align=\"left\">" + brief + "</align>", [HintEffectPresets.FadeOut()], 10f);
                return true;
            }

            response = "No upgrades available.";
            p.SendHint($"<align=\"left\">{response}</align>", [HintEffectPresets.FadeOut()], 2f);
            return false;
        }
    }
}
