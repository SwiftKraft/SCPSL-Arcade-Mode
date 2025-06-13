using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Trailblazer", Rarity.Rare)]
    public class Trailblazer(PerkInventory inv) : PerkCooldownBase(inv)
    {
        public override string Name => "Trailblazer";

        public override string Description => $"Set a teleport point after using an item. Teleport to the point after using an item of the same type, and remove the teleport point. No item types will be tracked when a teleport point exists. {base.Description}";

        public override float Cooldown => 80f;

        public Vector3 TeleportPoint;
        public bool TeleportExists;
        public ItemType TrackedType;

        public override void Effect()
        {
            if (!TeleportExists)
                return;

            Player.Position = TeleportPoint;
            TrackedType = ItemType.None;
            TeleportExists = false;
            SendMessage("Teleported! Teleport point destroyed.");
        }

        public override void Init()
        {
            base.Init();

            WarheadEvents.Detonated += OnWarheadDetonated;
            PlayerEvents.UsingItem += OnUsingItem;
            PlayerEvents.UsedItem += OnUsedItem;
        }

        private void OnWarheadDetonated(LabApi.Events.Arguments.WarheadEvents.WarheadDetonatedEventArgs ev)
        {
            Player.Position = TeleportPoint;
            TrackedType = ItemType.None;
            TeleportExists = false;
            SendMessage("Warhead detonated! Teleport point destroyed.");
        }

        public override void Remove()
        {
            base.Remove();

            WarheadEvents.Detonated -= OnWarheadDetonated;
            PlayerEvents.UsingItem -= OnUsingItem;
            PlayerEvents.UsedItem -= OnUsedItem;
        }

        protected virtual void OnUsingItem(PlayerUsingItemEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            if (TeleportExists && ev.UsableItem.Type == TrackedType)
                SendMessage($"{(CooldownTimer.Ended ? "<color=#00FF00>You will be teleported!</color>" : "<color=#FF0000>On cooldown, you will NOT be teleported!</color>")} Cancel to abort.");
        }

        protected virtual void OnUsedItem(PlayerUsedItemEventArgs ev) // Teleport point follows elevator
        {
            if (ev.Player != Player)
                return;

            if (TeleportExists)
            {
                if (ev.UsableItem.Type == TrackedType)
                    Trigger();
                return;
            }

            TeleportPoint = Player.Position;
            TrackedType = ev.UsableItem.Type;
            TeleportExists = true;
            SendMessage($"Teleport point has been created{(!CooldownTimer.Ended ? " (teleport on cooldown)" : "")}! Tracked type: " + Translations.Get(TrackedType));
        }
    }
}
