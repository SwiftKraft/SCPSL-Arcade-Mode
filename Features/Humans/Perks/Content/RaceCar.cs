using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("RaceCar", Rarity.Epic)]
    public class RaceCar(PerkInventory inv) : PerkKillBase(inv)
    {
        public override string Name => "Race Car";

        public override string Description => "Every kill increases your speed.";

        public virtual byte Amount => 20;

        byte currentStack;

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player)
                return;

            if (currentStack < byte.MaxValue)
            {
                currentStack = (byte)Mathf.Clamp(currentStack + Amount, 0, byte.MaxValue);
                Player.EnableEffect<MovementBoost>(currentStack);
            }
        }

        public override void Remove()
        {
            base.Remove();
            Player.DisableEffect<MovementBoost>();
        }
    }
}
