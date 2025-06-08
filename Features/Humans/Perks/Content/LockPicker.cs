using Interactables.Interobjects.DoorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("LockPicker", Rarity.Legendary)]
    public class LockPicker(PerkInventory inv) : PerkDoorBase(inv)
    {
        public override string Name => "Lock Picker";

        public override string Description => "Open any door, even locked ones.";

        public override void OnDoorAction(DoorVariant door, DoorAction act, ReferenceHub hub)
        {
            if (hub != Player.ReferenceHub || act == DoorAction.Destroyed || act == DoorAction.Opened || act == DoorAction.Closed)
                return;

            door.NetworkTargetState = true;
        }
    }
}
