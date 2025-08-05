using LabApi.Events.Handlers;
using MEC;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("AdvSlotUpgrade", Rarity.Mythic)]
    public class AdvancedPerkSlotUpgrade(PerkInventory inv) : PerkSlotUpgrade(inv)
    {
        public override string Name => "Advanced Perk Slot Upgrade";
        public override string Description => $"Gives you +{Amount} perk slots, takes up {SlotUsage} slots.\nGuaranteed loss of <color=red><b>ALL PERKS</b></color> when dying. \n{DropChance * 100f}% chance of dropping each removed perk, always drops itself.";

        public override int Amount => 12;
        public override int SlotUsage => 0;
        public virtual float DropChance => 0.3f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Dying += OnPlayerDying;
        }

        private void OnPlayerDying(LabApi.Events.Arguments.PlayerEvents.PlayerDyingEventArgs ev)
        {
            if (ev.Player != Player || !Player.TryGetPerkInventory(out PerkInventory inv))
                return;

            Vector3 pos = Player.Position;

            Timing.CallDelayed(0.2f, temp);

            void temp()
            {
                for (int i = inv.Perks.Count - 1; i >= 0; i--)
                {
                    Type removed = inv.Perks[i].GetType();
                    inv.RemovePerk(removed);
                    if (Random.Range(0f, 1f) <= DropChance || inv.Perks[i] == this)
                        PerkSpawner.SpawnPerk(PerkManager.GetPerk(removed), pos);
                }
            }
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Dying -= OnPlayerDying;
        }
    }
}
