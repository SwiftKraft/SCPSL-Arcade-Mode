using CommandSystem;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Features.Scoring;
using System;

namespace SwiftArcadeMode.Commands.Client
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ClientSeeScoreCommand : ICommand
    {
        public string Command => "seescore";

        public string[] Aliases => ["ss", "sscore"];

        public string Description => "Shows you your current score.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);

            response = $"Current score: {p.GetScore()}";
            return true;
        }
    }
}
