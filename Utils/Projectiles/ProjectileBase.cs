using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Utils.Projectiles
{
    public abstract class ProjectileBase
    {
        public abstract void Init();
        public abstract void Tick();
        public abstract void Destroy();
    }
}
