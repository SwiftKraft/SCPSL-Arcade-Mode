using LabApi.Events.Handlers;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("SuperRegeneration", Rarity.Epic)]
    public class SuperRegeneration(PerkInventory inv) : Regeneration(inv)
    {
        public override string Name => $"Super {base.Name}";

        public override string Description => $"{base.Description} However, max HP is decreased by {Decrease}.";

        public override float HealthThreshold => 20f;
        public override float Rate => 9f;
        public virtual float Decrease => 10f;

        float originalHealth;

        public override void Init()
        {
            base.Init();
            originalHealth = Player.MaxHealth;
            Player.MaxHealth = originalHealth - Decrease;

            PlayerEvents.ChangedRole += OnPlayerChangedRole;
        }

        private void OnPlayerChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            Player.MaxHealth = originalHealth - Decrease;
        }

        public override void Remove()
        {
            base.Remove();
            Player.MaxHealth = originalHealth;

            PlayerEvents.ChangedRole -= OnPlayerChangedRole;
        }
    }
}
