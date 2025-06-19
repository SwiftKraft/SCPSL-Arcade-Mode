using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SwiftUHC.Utils.Extensions;
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
        public Elevator TrackedElevator;

        public override void Effect()
        {
            if (!TeleportExists)
                return;

            Player.Position = TrackedElevator != null ? TrackedElevator.Base.transform.position + TeleportPoint : TeleportPoint;
            TrackedElevator = null;
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
            TrackedElevator = null;
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

            TrackedElevator = Player.GetElevator();
            TeleportPoint = TrackedElevator != null ? Player.Position - TrackedElevator.Base.transform.position : Player.Position;
            TrackedType = ev.UsableItem.Type;
            TeleportExists = true;
            SendMessage($"Teleport point has been created{(!CooldownTimer.Ended ? " (teleport on cooldown)" : "")}! Tracked type: " + Translations.Get(TrackedType));
        }
    }
}
