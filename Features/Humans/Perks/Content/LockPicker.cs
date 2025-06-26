using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Enums;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("LockPicker", Rarity.Legendary)]
    public class LockPicker(PerkInventory inv) : PerkDoorBase(inv)
    {
        public override string Name => "Lock Picker";

        public override string Description => "Open any door, even locked ones.";

        public override void OnDoorAction(DoorVariant door, DoorAction act, ReferenceHub hub)
        {
            if (door == null || hub != Player.ReferenceHub || act == DoorAction.Destroyed || act == DoorAction.Opened || act == DoorAction.Closed || door.DoorName.Equals(DoorName.Hcz079FirstGate.ToString()) || door.DoorName.Equals(DoorName.Hcz079SecondGate.ToString()) || door)
                return;

            door.NetworkTargetState = true;
        }
    }
}
