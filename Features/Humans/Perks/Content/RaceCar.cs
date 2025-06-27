using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("RaceCar", Rarity.Legendary)]
    public class RaceCar(PerkInventory inv) : PerkKillBase(inv)
    {
        public override string Name => "Race Car";

        public override string Description => "Every kill increases your speed.";

        public virtual byte Amount => 15;

        byte currentStack;

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player)
                return;

            if (currentStack < byte.MaxValue)
            {
                currentStack = (byte)Mathf.Clamp(currentStack + Amount, 0, byte.MaxValue);
                Player.EnableEffect<MovementBoost>(currentStack);
                SendMessage("Gained Kill, Speed Level: " + currentStack);
            }
        }

        public override void Remove()
        {
            base.Remove();
            Player.DisableEffect<MovementBoost>();
        }
    }
}
