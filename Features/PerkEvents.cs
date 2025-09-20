using LabApi.Events;
using SwiftArcadeMode.Features.Humans.Perks;

namespace SwiftArcadeMode.Features
{
    public static class PerkEvents
    {
        public static event LabEventHandler<AttemptAddEventArgs> AttemptAdd;
        public static event LabEventHandler<CheckPickupEventArgs> CheckPickup;
        public static event LabEventHandler<PickedUpPerkEventArgs> PickedUpPerk;

        public static void OnAttemptAdd(AttemptAddEventArgs ev) => AttemptAdd?.Invoke(ev);
        public static void OnCheckPickup(CheckPickupEventArgs ev) => CheckPickup?.Invoke(ev);
        public static void OnPickedUpPerk(PickedUpPerkEventArgs ev) => PickedUpPerk?.Invoke(ev);
    }
}
