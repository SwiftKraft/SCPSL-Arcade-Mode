using System.Collections.Generic;
using UnityEngine;

namespace SwiftUHC.Features.SCPs.Upgrades
{
    public abstract class UpgradePathPerkBase(PerkInventory inv) : PerkBase(inv)
    {
        public readonly List<UpgradeBase> Path = [];

        public int Progress { get => _progress; set => _progress = value; }
        int _progress;

        public override void Init()
        {
            base.Init();

            Progress = 1;
        }

        public override void Tick()
        {
            base.Tick();

            if (Path.Count <= 0)
                return;

            for (int i = 0; i < Mathf.Min(Progress, Path.Count); i++)
                Path[i].Tick();
        }

        public override void Remove()
        {
            base.Remove();

            if (Path.Count <= 0)
                return;

            for (int i = 0; i < Mathf.Min(Progress, Path.Count); i++)
                Path[i].Remove();
        }
    }

    public abstract class UpgradeBase
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual void Init() { }

        public virtual void Tick() { }

        public virtual void Remove() { }
    }
}
