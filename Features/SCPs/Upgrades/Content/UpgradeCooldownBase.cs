﻿using SwiftUHC.Utils.Structures;
using UnityEngine;

namespace SwiftUHC.Features.SCPs.Upgrades.Content
{
    public abstract class UpgradeCooldownBase<T>(UpgradePathPerkBase parent) : UpgradeBase<T>(parent) where T : UpgradePathPerkBase
    {
        public override string Description => $"{UpgradeDescription}\nCooldown: {Cooldown}s.";

        public abstract string UpgradeDescription { get; }

        public virtual string ReadyMessage => "Ready!";

        public virtual float Cooldown => 10f;
        protected Timer CooldownTimer = new();

        public override void Init()
        {
            base.Init();
            CooldownTimer.OnTimerEnd += OnCooldownEnd;
        }

        public override void Remove()
        {
            base.Remove();
            CooldownTimer.OnTimerEnd -= OnCooldownEnd;
        }

        public virtual void Trigger()
        {
            if (!CooldownTimer.Ended)
                return;

            Effect();
            CooldownTimer.Reset(Cooldown);
        }

        protected virtual void OnCooldownEnd()
        {
            if (!string.IsNullOrWhiteSpace(ReadyMessage))
                SendMessage(ReadyMessage);
        }

        public abstract void Effect();

        public override void Tick()
        {
            base.Tick();
            CooldownTimer.Tick(Time.fixedDeltaTime);
        }
    }
}
