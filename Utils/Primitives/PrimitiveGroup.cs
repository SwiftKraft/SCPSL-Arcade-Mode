using LabApi.Features.Wrappers;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftArcadeMode.Utils.Primitives
{
    public class PrimitiveGroup(params Primitive[] primitives)
    {
        public readonly List<Primitive> Toys = [.. primitives];
        public virtual void Spawn()
        {
            for (int i = 0; i < Toys.Count; i++)
                Toys[i].Spawn();
        }
        public virtual void Tick()
        {
            for (int i = 0; i < Toys.Count; i++)
                Toys[i].Tick();
        }
        public virtual void Destroy()
        {
            for (int i = 0; i < Toys.Count; i++)
                Toys[i].Destroy();
        }
    }

    public abstract class Primitive(PrimitiveType toy, Transform parent = null)
    {
        public PrimitiveObjectToy Toy { get; private set; }
        public readonly PrimitiveType Type = toy;
        public readonly Transform Parent = parent;

        public virtual void Spawn()
        {
            if (Toy != null)
                return;

            PrimitiveObjectToy t = PrimitiveObjectToy.Create(Parent, false);
            t.Type = Type;
            Toy = t;
            Init();
            t.Spawn();
        }

        public abstract void Init();
        public abstract void Tick();

        public virtual void Destroy() => Toy?.Destroy();
    }
}
