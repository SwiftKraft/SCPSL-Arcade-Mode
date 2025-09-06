using CustomPlayerEffects;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Ghostly", Rarity.Epic)]
    public class Ghostly(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Ghostly";

        public override string Description => "Phase through doors, you get more transparent the lower your health is.";

        public float HealthPercentage => Player.Health / Player.MaxHealth;

        float lastCheckedHealth;

        public override void Init()
        {
            base.Init();
            Player.EnableEffect<CustomPlayerEffects.Ghostly>();
            lastCheckedHealth = Player.Health;
        }

        public override void Tick()
        {
            base.Tick();
            if (lastCheckedHealth != Player.Health)
                Player.EnableEffect<Fade>((byte)Mathf.Lerp(255, 0, HealthPercentage));

            lastCheckedHealth = Player.Health;
        }

        public override void Remove()
        {
            base.Remove();
            Player.DisableEffect<CustomPlayerEffects.Ghostly>();
        }
    }
}
