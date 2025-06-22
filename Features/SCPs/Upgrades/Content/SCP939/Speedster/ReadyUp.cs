using CustomPlayerEffects;
using PlayerRoles.PlayableScps.Scp939;
using SwiftUHC.Utils.Structures;
using UnityEngine;

namespace SwiftUHC.Features.SCPs.Upgrades.Content.SCP939.Speedster
{
    public class ReadyUp(UpgradePathPerkBase parent) : UpgradeBase<Speedster>(parent)
    {
        public override string Name => "Ready Up";

        public override string Description => $"Crouching for {Requirement}s will grant you a speed boost for {Duration}s after standing up.";

        public virtual float Duration => 8f;

        public virtual float Requirement => 3f;

        public bool CrouchState => role != null && role.SubroutineModule.TryGetSubroutine(out Scp939FocusAbility focus) && focus.State > 0.5f;

        private readonly Timer timer = new();
        Scp939Role role;
        bool trigger;

        public override void Init()
        {
            base.Init();
            if (Player.RoleBase is Scp939Role r)
                role = r;

            timer.OnTimerEnd += OnTimerEnd;
        }

        private void OnTimerEnd()
        {
            SendMessage("Speed boost available!");
            trigger = true;
        }

        public override void Tick()
        {
            base.Tick();

            if (CrouchState)
            {
                SendMessage("Time until boost: " + Mathf.Round(timer.CurrentValue) + "s");
                timer.Tick(Time.fixedDeltaTime);
            }
            else
            {
                if (timer.Ended && trigger)
                {
                    trigger = false;
                    Player.EnableEffect<MovementBoost>(70, Duration);
                }

                timer.Reset(Requirement);
            }
        }

        public override void Remove()
        {
            base.Remove();
            timer.OnTimerEnd -= OnTimerEnd;
        }
    }
}
