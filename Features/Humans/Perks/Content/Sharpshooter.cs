using InventorySystem.Items;
using InventorySystem.Items.Firearms.Attachments;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Sharpshooter", Rarity.Uncommon)]
    public class Sharpshooter(PerkInventory inv) : PerkKillBase(inv)
    {
        public override string Name => "Sharpshooter";

        public override string Description => "Every firearm you pick up turns into a revolver. \nKills with the revolver grant you AHP.";

        public virtual float Amount => 20f;
        public virtual float Efficacy => 1f;

        AhpStat.AhpProcess ahp;

        public override void Init()
        {
            base.Init();

            PlayerEvents.PickedUpItem += OnPickedUpItem;

            ahp = Player.CreateAhpProcess(0f, 75f, 0f, Efficacy, 1f, true);

            Player.AddAmmo(ItemType.Ammo44cal, 50);
        }

        public override void Remove()
        {
            base.Remove();

            PlayerEvents.PickedUpItem -= OnPickedUpItem;

            Player.ServerKillProcess(ahp);
        }

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player || Player.CurrentItem == null || Player.CurrentItem.Type != ItemType.GunRevolver)
                return;

            ahp.CurrentAmount += Amount;
        }

        private void OnPickedUpItem(PlayerPickedUpItemEventArgs ev)
        {
            if (ev.Player != Player || ev.Item.Category != ItemCategory.Firearm || ev.Item.Type == ItemType.GunRevolver)
                return;

            Player.RemoveItem(ev.Item);
            FirearmItem it = Player.AddItem(ItemType.GunRevolver, ItemAddReason.PickedUp) as FirearmItem;
            Player.AddAmmo(ItemType.Ammo44cal, 30);
            it.Base.ApplyAttachmentsCode(AttachmentsServerHandler.PlayerPreferences[Player.ReferenceHub][ItemType.GunRevolver], true);
        }
    }
}
