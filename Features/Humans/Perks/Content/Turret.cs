using Achievements.Handlers;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using PlayerStatsSystem;
using SwiftUHC.Utils.Structures;
using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Turret", Rarity.Rare)]
    public class Turret(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Turret";

        public override string Description => $"Aiming down sights for at least {Duration} seconds will increase your damage by x{Multiplier}, but you cannot move.";

        public virtual float Duration => 0.5f;
        public virtual float Multiplier => 2f;

        public bool AimStatus { get; private set; }

        public bool Effect
        {
            get => effect;
            private set
            {
                if (effect == value)
                    return;

                effect = value;

                if (effect)
                {
                    originallyEnsnared = Player.GetEffect<Ensnared>().IsEnabled;
                    Player.EnableEffect<Ensnared>();
                }
                else
                {
                    if (!originallyEnsnared)
                        Player.DisableEffect<Ensnared>();
                    else
                        Player.EnableEffect<Ensnared>();
                }
            }
        }

        readonly Timer timer = new();
        private bool effect;
        private bool originallyEnsnared;

        public override void Init()
        {
            base.Init();
            PlayerEvents.AimedWeapon += OnAimedWeapon;
            PlayerEvents.ChangedItem += OnChangedItem;
            PlayerEvents.DroppingItem += OnDroppingItem;
            PlayerEvents.Hurting += OnHurting;
        }

        public override void Tick()
        {
            base.Tick();
            if (AimStatus)
            {
                timer.Tick(Time.fixedDeltaTime);
                if (timer.Ended)
                    Effect = true;
            }
            else
                Effect = false;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.AimedWeapon -= OnAimedWeapon;
            PlayerEvents.ChangedItem -= OnChangedItem;
            PlayerEvents.DroppingItem -= OnDroppingItem;
            PlayerEvents.Hurting -= OnHurting;
        }

        private void OnHurting(PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker != Player || !Effect)
                return;

            if (ev.DamageHandler is StandardDamageHandler stn)
                stn.Damage *= Multiplier;
        }

        private void OnDroppingItem(PlayerDroppingItemEventArgs ev)
        {
            if (ev.Player != Player || ev.Item != Player.CurrentItem)
                return;
            AimStatus = false;
        }

        private void OnChangedItem(PlayerChangedItemEventArgs ev)
        {
            if (ev.Player != Player)
                return;
            AimStatus = false;
        }

        private void OnAimedWeapon(PlayerAimedWeaponEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            timer.Reset(Duration);
            AimStatus = ev.Aiming;
        }
    }
}
