using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.ArmyBuilder
{
    public class EfficientReanimation(UpgradePathPerkBase parent) : UpgradeBase<ArmyBuilder>(parent)
    {
        public override string Name => "Efficient Reanimation";

        public override string Description => $"Revived zombies no longer receive max health debuffs, all zombies have {MaxHealth} max HP.";

        public virtual float MaxHealth => 600f;

        public override void Init()
        {
            base.Init();
            Parent.OnAddedZombie += OnAddedZombie;
        }

        public override void Remove()
        {
            base.Remove();
            Parent.OnAddedZombie -= OnAddedZombie;
        }

        private void OnAddedZombie(Player obj)
        {
            obj.MaxHealth = MaxHealth;
            obj.Health = MaxHealth;
        }
    }
}
