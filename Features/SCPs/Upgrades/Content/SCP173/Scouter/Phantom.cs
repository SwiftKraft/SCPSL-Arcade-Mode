using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using NetworkManagerUtils.Dummies;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace SwiftUHC.Features.SCPs.Upgrades.Content.SCP173.Scouter
{
    public class Phantom(UpgradePathPerkBase parent) : UpgradeBase<Scouter>(parent)
    {
        public override string Name => "Phantom";

        public override string Description => "Spawns a phantom 173 when you start breakneck speeds.";

        ReferenceHub phantom;

        public override void Init()
        {
            base.Init();
            Scp173Events.BreakneckSpeedChanged += OnBreakneckSpeedChanged;
            PlayerEvents.Hurt += OnHurt;
            PlayerEvents.Left += OnLeft;
        }

        public override void Remove()
        {
            base.Remove();
            Scp173Events.BreakneckSpeedChanged -= OnBreakneckSpeedChanged;
            PlayerEvents.Hurt -= OnHurt;
            PlayerEvents.Left -= OnLeft;

            DeletePhantom();
        }

        private void OnHurt(LabApi.Events.Arguments.PlayerEvents.PlayerHurtEventArgs ev)
        {
            if (ev.Player.ReferenceHub != phantom)
                return;

            DeletePhantom(ev.Attacker);
        }

        private void OnLeft(LabApi.Events.Arguments.PlayerEvents.PlayerLeftEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            DeletePhantom();
        }

        private void OnBreakneckSpeedChanged(LabApi.Events.Arguments.Scp173Events.Scp173BreakneckSpeedChangedEventArgs ev)
        {
            if (ev.Player != Player || !ev.Active)
                return;

            Vector3 pos = Player.Position;

            if (phantom == null)
                phantom = DummyUtils.SpawnDummy(Player.DisplayName + " (Phantom)");

            Timing.CallDelayed(0.1f, () =>
            {
                phantom.roleManager.ServerSetRole(RoleTypeId.Scp173, RoleChangeReason.RemoteAdmin);
                phantom.TryOverridePosition(pos);
            });
        }

        public void DeletePhantom(Player attacker = null)
        {
            if (phantom == null)
                return;

            SendMessage("Phantom destroyed! " + (attacker == null ? "" : "Attacker: " + attacker.DisplayName));
            TimedGrenadeProjectile.SpawnActive(phantom.GetPosition(), ItemType.GrenadeFlash, Player, 0.1f);

            if (Room.TryGetRoomAtPosition(phantom.GetPosition(), out Room room))
                room.LightController.FlickerLights(5f);

            phantom.roleManager.ServerSetRole(RoleTypeId.Filmmaker, RoleChangeReason.RemoteAdmin);
        }
    }
}
