﻿using CustomPlayerEffects;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using MEC;
using SwiftArcadeMode.Features;
using SwiftArcadeMode.Features.Game.Modes;
using SwiftArcadeMode.Features.Humans.Perks;
using SwiftArcadeMode.Features.Humans.Perks.Crafting;
using SwiftArcadeMode.Features.Scoring;
using SwiftArcadeMode.Features.Scoring.Saving;
using SwiftArcadeMode.Features.SCPs.Upgrades;
using SwiftArcadeMode.Features.ServerSpecificSettings;
using SwiftArcadeMode.Utils.Projectiles;
using System;
using System.IO;
using Logger = LabApi.Features.Console.Logger;

namespace SwiftArcadeMode
{
    public class Core : Plugin<Config>
    {
        public static Core Instance { get; private set; }

        public override string Name => "SCPSL Arcade Mode";

        public override string Description => "Adds various interesting and fun mechanics to the game! ";

        public override string Author => "SwiftKraft";

        public override Version Version => new(2, 1, 0);

        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public override void Enable()
        {
            Logger.Info($"Arcade Mode {Version} by SwiftKraft: Loaded!");

            SaveManager.SaveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SCP Secret Laboratory", "Swift Arcade Mode", "Scoring");
            SaveManager.SaveFileName = "scores.txt";
            SaveManager.SavePath = Path.Combine(SaveManager.SaveDirectory, SaveManager.SaveFileName);

            Logger.Info($"Scoring save file path: {SaveManager.SavePath}");

            Instance = this;

            StaticUnityMethods.OnFixedUpdate += FixedUpdate;
            PerkManager.Enable();
            PerkSpawner.Enable();
            UpgradePathManager.Enable();
            UpgradePathGiver.Enable();
            ScoringManager.Enable();
            RecipeManager.Enable();
            SSSManager.Enable();
            GameModeManager.Enable();

            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundStarted += OnRoundStarted;
            ServerEvents.RoundRestarted += OnRoundRestarted;
            ServerEvents.MapGenerated += OnMapGenerated;
            Shutdown.OnQuit += OnQuit;
            PlayerEvents.UpdatedEffect += OnUpdatedEffect;
            PlayerEvents.ChangedRole += OnChangedRole;
            PlayerEvents.Left += OnLeft;

            SaveManager.LoadScores();
        }

        private void OnRoundRestarted() => SaveManager.SaveScores();

        private void OnLeft(LabApi.Events.Arguments.PlayerEvents.PlayerLeftEventArgs ev)
        {
            if (ev.Player.TryGetPerkInventory(out PerkInventory inv))
                inv.ClearPerks();
        }

        private void OnMapGenerated(LabApi.Events.Arguments.ServerEvents.MapGeneratedEventArgs ev) => SaveManager.LoadScores();

        private void OnRoundStarted() => SaveManager.LoadScores();

        private void OnRoundEnded(LabApi.Events.Arguments.ServerEvents.RoundEndedEventArgs ev) => SaveManager.SaveScores();

        private void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (Config.Replace096 && ev.NewRole.RoleTypeId == PlayerRoles.RoleTypeId.Scp096 && ev.ChangeReason == PlayerRoles.RoleChangeReason.RoundStart)
                Timing.CallDelayed(0.1f, () => ev.Player.SetRole(PlayerRoles.RoleTypeId.Scp3114));
        }

        private void OnUpdatedEffect(LabApi.Events.Arguments.PlayerEvents.PlayerEffectUpdatedEventArgs ev)
        {
            if (Config.SkeletonBalance && ev.Effect is Strangled str)
                str.ServerChangeDuration(2f);
        }

        public override void Disable()
        {
            StaticUnityMethods.OnFixedUpdate -= FixedUpdate;
            PerkManager.Disable();
            PerkSpawner.Disable();
            UpgradePathManager.Disable();
            UpgradePathGiver.Disable();
            ScoringManager.Disable();
            SSSManager.Disable();
            GameModeManager.Disable();

            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundStarted -= OnRoundStarted;
            ServerEvents.RoundRestarted -= OnRoundRestarted;
            ServerEvents.MapGenerated -= OnMapGenerated;
            Shutdown.OnQuit -= OnQuit;
            PlayerEvents.UpdatedEffect -= OnUpdatedEffect;
            PlayerEvents.ChangedRole -= OnChangedRole;
            PlayerEvents.Left -= OnLeft;

            SaveManager.SaveScores();
        }

        private void OnQuit() => SaveManager.SaveScores();

        public static void FixedUpdate()
        {
            GameModeManager.Tick();
            PerkManager.Tick();
            ProjectileManager.Tick();
            ScoringManager.Tick();
        }

        /*
        Ideas:

        Random round events and mini modes.
        */
    }
}
