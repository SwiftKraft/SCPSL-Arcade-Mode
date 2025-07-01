﻿using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using System.Linq;
using UnityEngine;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.TeamPlayer
{
    public class LifeConsumption(UpgradePathPerkBase parent) : UpgradeCooldownTriggerBase<TeamPlayer>(parent)
    {
        public override string Name => "Life Consumption";

        public override string Description => $"For each human you have in the pocket dimension, +{Amount} HS every second.";

        public virtual float Amount => 5f;

        public override string UpgradeDescription => "";

        public override string ReadyMessage => "";

        public override float Cooldown => 1f;

        public float CurrentAmount => Amount * Player.List.Count((p) => p.HasEffect<PocketCorroding>());

        public override void Effect()
        {
            if (Player.HumeShield < Player.MaxHumeShield)
                Player.HumeShield = Mathf.Clamp(Player.HumeShield + CurrentAmount, 0f, Player.MaxHumeShield);
        }
    }
}
