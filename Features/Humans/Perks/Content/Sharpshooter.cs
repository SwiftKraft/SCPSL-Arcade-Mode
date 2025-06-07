using InventorySystem.Items;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Sharpshooter", Rarity.Rare)]
    public class Sharpshooter(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Sharpshooter";

        public override string Description => "Every weapon you pick up turns into a revolver. No running inaccuracy when using a revolver.";

        public override void Init()
        {
            base.Init();

            PlayerEvents.PickedUpItem += OnPickedUpItem;
            PlayerEvents.ChangedItem += OnChangedItem;
            PlayerEvents.DroppingItem += OnDroppingItem;
        }

        public override void Remove()
        {
            base.Remove();

            PlayerEvents.PickedUpItem -= OnPickedUpItem;
            PlayerEvents.ChangedItem -= OnChangedItem;
            PlayerEvents.DroppingItem -= OnDroppingItem;
        }

        float originalBaseBulletInaccuracy;
        float originalMinPenalty;
        float originalMaxPenalty;
        float originalBaseDamage;

        private void OnChangedItem(PlayerChangedItemEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            if (ev.NewItem != null && ev.NewItem.Type == ItemType.GunRevolver && ev.NewItem is FirearmItem f && f.Base.TryGetModule(out SingleBulletHitscan mod) && f.Base.TryGetModule(out MovementInaccuracyModule mod2))
            {
                originalBaseBulletInaccuracy = mod.BaseBulletInaccuracy;
                originalMinPenalty = mod2.MinPenalty;
                originalMaxPenalty = mod2.MaxPenalty;
                originalBaseDamage = mod.BaseDamage;

                mod.BaseBulletInaccuracy = 0f;
                mod2.MinPenalty = 0f;
                mod2.MaxPenalty = 0f;
                mod.BaseDamage = 500f;
            }

            if (ev.OldItem != null && ev.OldItem.Type == ItemType.GunRevolver && ev.OldItem is FirearmItem f2 && f2.Base.TryGetModule(out mod) && f2.Base.TryGetModule(out mod2))
            {
                mod.BaseBulletInaccuracy = originalBaseBulletInaccuracy;
                mod2.MinPenalty = originalMinPenalty;
                mod2.MaxPenalty = originalMaxPenalty;
                mod.BaseDamage = originalBaseDamage;
            }
        }

        private void OnDroppingItem(PlayerDroppingItemEventArgs ev)
        {
            if (ev.Player != Player || ev.Item != Player.CurrentItem || ev.Item.Category != ItemCategory.Firearm || ev.Item.Type != ItemType.GunRevolver)
                return;

            if (ev.Item.Type == ItemType.GunRevolver && ev.Item is FirearmItem f && f.Base.TryGetModule(out SingleBulletHitscan mod) && f.Base.TryGetModule(out MovementInaccuracyModule mod2))
            {
                mod.BaseBulletInaccuracy = originalBaseBulletInaccuracy;
                mod2.MinPenalty = originalMinPenalty;
                mod2.MaxPenalty = originalMaxPenalty;
                mod.BaseDamage = originalBaseDamage;
            }
        }

        private void OnPickedUpItem(PlayerPickedUpItemEventArgs ev)
        {
            if (ev.Player != Player || ev.Item.Category != ItemCategory.Firearm || ev.Item.Type == ItemType.GunRevolver)
                return;

            Player.RemoveItem(ev.Item);
            Player.AddItem(ItemType.GunRevolver, ItemAddReason.PickedUp);
        }
    }
}
