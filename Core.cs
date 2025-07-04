﻿using CustomPlayerEffects;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using MEC;
using SwiftArcadeMode.Features;
using SwiftArcadeMode.Features.Humans.Perks;
using SwiftArcadeMode.Features.Scoring;
using SwiftArcadeMode.Features.Scoring.Saving;
using SwiftArcadeMode.Features.SCPs.Upgrades;
using SwiftArcadeMode.ServerSpecificSettings;
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

        public override Version Version => new(1, 7, 0);

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
            SSSManager.Enable();
            ScoringManager.Enable();

            ServerEvents.RoundEnded += OnRoundEnded;
            ServerEvents.RoundStarted += OnRoundStarted;
            ServerEvents.MapGenerated += OnMapGenerated;
            Shutdown.OnQuit += OnQuit;
            PlayerEvents.UpdatedEffect += OnUpdatedEffect;
            PlayerEvents.ChangedRole += OnChangedRole;
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
            if (Config.SkeletonBalance && ev.Effect is Strangled)
                Timing.CallDelayed(1f, ev.Player.DisableEffect<Strangled>);
        }

        public override void Disable()
        {
            StaticUnityMethods.OnFixedUpdate -= FixedUpdate;
            PerkManager.Disable();
            PerkSpawner.Disable();
            UpgradePathManager.Disable();
            UpgradePathGiver.Disable();
            SSSManager.Disable();
            ScoringManager.Disable();

            ServerEvents.RoundEnded -= OnRoundEnded;
            ServerEvents.RoundStarted -= OnRoundStarted;
            ServerEvents.MapGenerated -= OnMapGenerated;
            Shutdown.OnQuit -= OnQuit;
            PlayerEvents.UpdatedEffect -= OnUpdatedEffect;
            PlayerEvents.ChangedRole -= OnChangedRole;

            SaveManager.SaveScores();
        }

        private void OnQuit() => SaveManager.SaveScores();

        public static void FixedUpdate()
        {
            PerkManager.Tick();
            SSSManager.Tick();
            ScoringManager.Tick();
        }

        /*
        Ideas:

        Random round events and mini modes.
        Perk crafting
        */
    }
}
