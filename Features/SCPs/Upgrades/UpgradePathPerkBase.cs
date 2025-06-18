using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftUHC.Features.SCPs.Upgrades
{
    public abstract class UpgradePathPerkBase(PerkInventory inv) : PerkBase(inv)
    {
        public readonly List<UpgradeBase> Path = [];

        public int Progress
        {
            get => _progress;
            set
            {
                if (Path.Count <= 0)
                    return;

                value = Mathf.Clamp(value, 0, Path.Count - 1);
                _progress = Mathf.Clamp(_progress, 0, Path.Count - 1);

                if (_progress == value)
                    return;

                if (value > _progress)
                {
                    for (int i = _progress + 1; i <= value; i++)
                        Path[i].Init();
                }
                else
                {
                    for (int i = value + 1; i <= _progress; i++)
                        Path[i].Remove();
                }

                _progress = value;
            }
        }
        int _progress;

        public abstract Type[] AllUpgrades { get; }

        public override void Init()
        {
            base.Init();

            foreach (Type upgrade in AllUpgrades)
            {
                if (upgrade.IsAbstract || !upgrade.IsAssignableFrom(typeof(UpgradeBase)))
                    continue;

                UpgradeBase b = (UpgradeBase)Activator.CreateInstance(upgrade, this);
                Path.Add(b);
            }

            Progress = 1;
        }

        public override void Tick()
        {
            base.Tick();

            if (Path.Count <= 0)
                return;

            for (int i = 0; i < Progress; i++)
                Path[i].Tick();
        }

        public override void Remove()
        {
            base.Remove();

            if (Path.Count <= 0)
                return;

            for (int i = 0; i < Progress; i++)
                Path[i].Remove();
        }
    }

    public abstract class UpgradeBase(UpgradePathPerkBase parent)
    {
        public readonly UpgradePathPerkBase Parent = parent;

        public Player Player => Parent.Player;

        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual void Init() { }

        public virtual void Tick() { }

        public virtual void Remove() { }
    }

    public abstract class UpgradeBase<T>(UpgradePathPerkBase parent) : UpgradeBase(parent) where T : UpgradePathPerkBase
    {
        public new T Parent => base.Parent is T t ? t : null;
    }
}
