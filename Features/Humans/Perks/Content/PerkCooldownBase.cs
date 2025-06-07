using SwiftUHC.Utils.Structures;
using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    public abstract class PerkCooldownBase(PerkInventory inv) : PerkBase(inv)
    {
        public override string Description => $"Cooldown: {Cooldown}s.";

        public virtual float Cooldown => 10f;
        protected Timer CooldownTimer = new();

        public virtual void Trigger()
        {
            if (!CooldownTimer.Ended)
                return;

            Effect();
            CooldownTimer.Reset(Cooldown);
        }

        public abstract void Effect();

        public override void Tick()
        {
            base.Tick();
            CooldownTimer.Tick(Time.fixedDeltaTime);
        }
    }
}
