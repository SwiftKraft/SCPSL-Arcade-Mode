using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

namespace SwiftUHC.Features.Humans.Perks.Content
{
    public abstract class PerkKillBase(PerkInventory inv) : PerkBase(inv)
    {
        public override void Init()
        {
            base.Init();
            PlayerEvents.Dying += OnPlayerDying;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Dying -= OnPlayerDying;
        }

        protected abstract void OnPlayerDying(PlayerDyingEventArgs ev);
    }
}
