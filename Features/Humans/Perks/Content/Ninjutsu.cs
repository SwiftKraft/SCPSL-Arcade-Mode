﻿using CustomPlayerEffects;
using SwiftUHC.Utils.Structures;
using UnityEngine;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    [Perk("Ninjutsu", Rarity.Rare)]
    public class Ninjutsu(PerkInventory inv) : PerkCooldownBase(inv)
    {
        public override string Name => "Ninjutsu";

        public override string Description => $"Turns you invisible at <{HealthThreshold} HP for {Duration}s. {base.Description}";

        public virtual float HealthThreshold => 20f;

        public virtual float Duration => 5f;

        public override float Cooldown => 120f;

        private readonly Timer activationTimer = new();

        public override void Init()
        {
            base.Init();
            activationTimer.OnTimerEnd += OnTimerEnd;
        }

        public override void Remove()
        {
            base.Remove();
            activationTimer.OnTimerEnd -= OnTimerEnd;
        }

        protected virtual void OnTimerEnd() => Player.DisableEffect<Invisible>();

        public override void Effect()
        {
            Player.EnableEffect<Invisible>();
            Player.CurrentItem = null;
            activationTimer.Reset(Duration);
        }

        public override void Tick()
        {
            base.Tick();

            if (Player.Health <= HealthThreshold)
                Trigger();

            if (!activationTimer.Ended)
                SendMessage("Invisible For: " + Mathf.Round(activationTimer.CurrentValue) + "s");

            activationTimer.Tick(Time.fixedDeltaTime);
        }
    }
}
