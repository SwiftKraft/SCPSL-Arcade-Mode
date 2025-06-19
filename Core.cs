using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using SwiftUHC.Features;
using SwiftUHC.Features.Humans.Perks;
using SwiftUHC.Features.SCPs.Upgrades;
using System;

namespace SwiftUHC
{
    public class Core : Plugin
    {
        public override string Name => "SCPSL Ultra Hardcore";

        public override string Description => "Adds various interesting and fun mechanics to the game! ";

        public override string Author => "SwiftKraft";

        public override Version Version => new(0, 1, 0);

        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public override void Enable()
        {
            Logger.Info("Ultra Hardcore by SwiftKraft: Loaded!");

            StaticUnityMethods.OnFixedUpdate += FixedUpdate;
            PerkManager.Enable();
            PerkSpawner.Enable();
            UpgradePathManager.Enable();
            UpgradePathGiver.Enable();
        }

        public override void Disable()
        {
            StaticUnityMethods.OnFixedUpdate -= FixedUpdate;
            PerkManager.Disable();
            PerkSpawner.Disable();
            UpgradePathManager.Disable();
            UpgradePathGiver.Disable();
        }

        public static void FixedUpdate() => PerkManager.Tick();
    }
}
