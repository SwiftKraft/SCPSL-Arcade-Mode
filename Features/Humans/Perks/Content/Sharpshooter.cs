using InventorySystem.Items;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Sharpshooter", Rarity.Rare)]
    public class Sharpshooter(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Sharpshooter";

        public override string Description => "Every weapon you pick up turns into a revolver. Kills with the revolver grant you AHP.";

        public virtual float Amount => 20f;
        public virtual float Efficacy => 1f;

        AhpStat.AhpProcess ahp;

        public override void Init()
        {
            base.Init();

            PlayerEvents.PickedUpItem += OnPickedUpItem;
            PlayerEvents.Dying += OnPlayerDying;

            ahp = Player.CreateAhpProcess(0f, 75f, 0f, Efficacy, 1f, true);

            Player.AddAmmo(ItemType.Ammo44cal, 50);
        }

        public override void Remove()
        {
            base.Remove();

            PlayerEvents.PickedUpItem -= OnPickedUpItem;
            PlayerEvents.Dying -= OnPlayerDying;

            Player.ServerKillProcess(ahp);
        }

        private void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player || Player.CurrentItem.Type != ItemType.GunRevolver)
                return;

            ahp.CurrentAmount += Amount;
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
