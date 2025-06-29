using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerRoles;
using SwiftArcadeMode.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Mitosis", Rarity.Epic)]
    public class Mitosis(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Mitosis";

        public override string Description => $"Spawn a teammate with your inventory when you use a healing item at max health. \nLimited to {Limit}.";

        public virtual int Limit => 2;

        public readonly List<Player> Spawned = [];

        public override void Init()
        {
            base.Init();
            PlayerEvents.UsedItem += OnUsedItem;
            PlayerEvents.ChangedRole += OnChangedRole;
        }

        private void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            if (Spawned.Contains(ev.Player) && !ev.Player.IsAlive)
                Spawned.Remove(ev.Player);
        }

        private void OnUsedItem(PlayerUsedItemEventArgs ev)
        {
            if (ev.Player != Player || Spawned.Count >= Limit || Player.HasEffect<PocketCorroding>() || ev.UsableItem.Category != ItemCategory.Medical || Player.Health < Player.MaxHealth)
                return;

            Player target = Player.List.Where(p => p.Role == RoleTypeId.Spectator).ToArray().GetRandom();
            target.SetRole(Player.Role);
            target.Position = Player.Position;
            target.ClearInventory();

            foreach (Item it in Player.Items)
                target.AddItem(it.Type);

            Spawned.Add(target);
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.UsedItem -= OnUsedItem;
            PlayerEvents.ChangedRole -= OnChangedRole;
        }
    }
}
