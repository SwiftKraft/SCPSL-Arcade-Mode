using LabApi.Features.Wrappers;
using System.Linq;

namespace SwiftArcadeMode.Utils.Extensions
{
    public static class PlayerExtensions
    {
        public static Elevator GetElevator(this Player player)
        {
            foreach (Elevator e in Elevator.List.Where(e => e.WorldSpaceRelativeBounds.Bounds.Contains(player.Position)))
                return e;
            return null;
        }
    }
}
