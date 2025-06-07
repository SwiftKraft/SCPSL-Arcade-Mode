namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("SuperRegeneration", Rarity.Rare)]
    public class SuperRegeneration(PerkInventory inv) : Regeneration(inv)
    {
        public override string Name => $"Super {base.Name}";

        public override string Description => $"{base.Description} However, max HP is decreased by {Decrease}.";

        public override float HealthThreshold => 20f;
        public override float Rate => 6f;
        public virtual float Decrease => 15f;

        float originalHealth;

        public override void Init()
        {
            base.Init();
            originalHealth = Player.MaxHealth;
            Player.MaxHealth = originalHealth - Decrease;
        }

        public override void Remove()
        {
            base.Remove();
            Player.MaxHealth = originalHealth;
        }
    }
}
