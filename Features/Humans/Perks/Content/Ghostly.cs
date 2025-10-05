using CustomPlayerEffects;
using LabApi.Events.Handlers;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Ghostly", Rarity.Epic)]
    public class Ghostly(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Ghostly";

        public override string Description => "Phase through doors, you get more transparent the lower your health is.\nStanding still will make you invisible.";

        public float HealthPercentage => Player.Health / Player.MaxHealth;

        Vector3 lastCheckedPosition;
        private byte currentDegree;

        public byte CurrentDegree
        {
            get => currentDegree;
            private set
            {
                if (currentDegree != value)
                    Player.EnableEffect<Fade>();

                currentDegree = value;
            }
        }

        public float Moving { get; private set; }

        public override void Init()
        {
            base.Init();
            PlayerEvents.ChangedRole += OnChangedRole;
            Player.EnableEffect<CustomPlayerEffects.Ghostly>();
            lastCheckedPosition = Player.Position;
        }

        private void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            Player.EnableEffect<CustomPlayerEffects.Ghostly>();
            lastCheckedPosition = Player.Position;
        }

        public override void Tick()
        {
            base.Tick();

            CurrentDegree = (byte)Mathf.Lerp(255, Mathf.Lerp(255, 0, HealthPercentage), Moving);
            Moving = Mathf.MoveTowards(Moving, lastCheckedPosition != Player.Position ? 0f : 1f, Time.fixedDeltaTime * 0.5f);

            lastCheckedPosition = Player.Position;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.ChangedRole -= OnChangedRole;
            Player.DisableEffect<CustomPlayerEffects.Ghostly>();
        }
    }
}
