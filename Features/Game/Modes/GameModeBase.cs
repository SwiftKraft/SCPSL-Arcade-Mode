namespace SwiftArcadeMode.Features.Game.Modes
{
    public abstract class GameModeBase
    {
        public abstract void Start();
        public abstract void Tick();
        public abstract void End();
    }
}
