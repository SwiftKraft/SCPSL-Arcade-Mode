using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftUHC.Features.SCPs.Upgrades
{
    public class UpgradeQueue(Player p)
    {
        public readonly Player Parent = p;

        public readonly Queue<Item> Upgrades = [];

        public struct Item(params UpgradePathAttribute[] choices)
        {
            public UpgradePathAttribute[] Choices = choices;
        }

        public void Create(int amount)
        {
            UpgradePathAttribute[] paths = new UpgradePathAttribute[amount];
            for (int i = 0; i < paths.Length; i++)
                paths[i] = Parent.Role.GetRandomUpgradePath(paths);
            Upgrades.Enqueue(new(paths));
        }


    }
}
