using PlayerRoles.PlayableScps.Scp939;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftUHC.Features.SCPs.Upgrades.Content.SCP939.Lurker
{
    public class CeilingStalker(UpgradePathPerkBase parent) : UpgradeBase<Lurker>(parent)
    {
        public override string Name => "Ceiling Stalker";

        public override string Description => "Focusing puts you on the ceiling.";

        public bool IsCrouching => focus != null && focus.State > 0.3f;

        public bool Active
        {
            get => active;
            set
            {
                if (active == value)
                    return;

                if (value)
                {
                    originalGravity = Player.Gravity;
                    Player.Gravity = -Player.Gravity;
                    Player.Scale = new(1f, -1f, 1f);
                }
                else
                {
                    Player.Gravity = originalGravity;
                    Player.Scale = Vector3.one;
                }

                active = value;
            }
        }

        private Vector3 originalGravity;
        private Scp939FocusAbility focus;
        private bool active;

        public override void Init()
        {
            base.Init();
            if (Player.RoleBase is Scp939Role role)
                role.SubroutineModule.TryGetSubroutine(out focus);
        }

        public override void Tick()
        {
            base.Tick();

            if (focus == null)
                return;

            Active = IsCrouching && Player.Zone != MapGeneration.FacilityZone.Surface;
        }

        public override void Remove()
        {
            base.Remove();
            Active = false;
        }
    }
}
