using System.Collections.Generic;

namespace SwiftArcadeMode.Utils.Projectiles
{
    public static class ProjectileManager
    {
        public readonly static List<ProjectileBase> All = [];

        public static void Enable() { }
        public static void Tick()
        {
            for (int i = 0; i < All.Count; i++)
                All[i].Tick();
        }
        public static void Disable() { }
    }
}
