using CustomPlayerEffects;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Features.Wrappers;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("FastHands", Rarity.Common)]
    public class FastHands(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Fast Hands";

        public override string Description => "Reload and unload weapons faster.";

        public bool Reloading
        {
            get => _reloading;
            private set
            {
                if (value == _reloading)
                    return;

                _reloading = value;

                if (_reloading)
                    originalIntensity = Player.GetEffect<Scp1853>().Intensity;

                Player.EnableEffect<Scp1853>(_reloading ? (byte)50 : originalIntensity);
            }
        }

        bool _reloading;
        byte originalIntensity;

        public override void Tick()
        {
            base.Tick();
            Reloading = Player.CurrentItem is FirearmItem f && f.Base.TryGetModule(out IReloaderModule mod) && mod.IsReloadingOrUnloading;
        }
    }
}
