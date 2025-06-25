using CustomPlayerEffects;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using MEC;
using SwiftUHC.Features;
using SwiftUHC.Features.Humans.Perks;
using SwiftUHC.Features.SCPs.Upgrades;
using SwiftUHC.ServerSpecificSettings;
using System;

namespace SwiftUHC
{
    public class Core : Plugin<Config>
    {
        public static Core Instance { get; private set; }

        public override string Name => "SCPSL Arcade Mode";

        public override string Description => "Adds various interesting and fun mechanics to the game! ";

        public override string Author => "SwiftKraft";

        public override Version Version => new(1, 5, 0);

        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public override void Enable()
        {
            Logger.Info($"Arcade Mode {Version} by SwiftKraft: Loaded!");

            Instance = this;

            StaticUnityMethods.OnFixedUpdate += FixedUpdate;
            PerkManager.Enable();
            PerkSpawner.Enable();
            UpgradePathManager.Enable();
            UpgradePathGiver.Enable();
            SSSManager.Enable();

            PlayerEvents.UpdatedEffect += OnUpdatedEffect;
            PlayerEvents.ChangedRole += OnChangedRole;
        }

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

            PlayerEvents.UpdatedEffect -= OnUpdatedEffect;
            PlayerEvents.ChangedRole -= OnChangedRole;
        }

        public static void FixedUpdate()
        {
            PerkManager.Tick();
            SSSManager.Tick();
        }

        /*
        Ideas:

        Random round events and mini modes.
        Perk crafting
        */
    }
}
