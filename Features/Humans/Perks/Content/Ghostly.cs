using CustomPlayerEffects;
using LabApi.Events.Handlers;
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
            PlayerEvents.ChangedRole += OnChangedRole;
            Player.EnableEffect<CustomPlayerEffects.Ghostly>();
            lastCheckedHealth = Player.Health;
        }

        private void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player)
                return;

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
            PlayerEvents.ChangedRole -= OnChangedRole;
            Player.DisableEffect<CustomPlayerEffects.Ghostly>();
        }
    }
}
