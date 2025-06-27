﻿using CommandSystem;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features;
using SwiftArcadeMode.Features.Humans.Perks;
using System;

namespace SwiftArcadeMode.Commands.RA
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class DropPerkCommand : ICommand
    {
        public string Command => "dropperk";

        public string[] Aliases => ["dperk", "dropp", "dp"];

        public string Description => "Drops a perk.";

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

            PerkSpawner.SpawnPerk(t, p.Position);

            response = "Dropped perk: " + t.ID;
            return true;
        }
    }
}
