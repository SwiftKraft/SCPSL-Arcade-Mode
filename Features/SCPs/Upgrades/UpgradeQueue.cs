using Hints;
using LabApi.Features.Wrappers;
using System.Collections.Generic;
using System.Text;

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

        public void Create(int amount, List<UpgradePathAttribute> maxed)
        {
            List<UpgradePathAttribute> paths = maxed;
            for (int i = 0; i < amount; i++)
            {
                UpgradePathAttribute att = Parent.Role.GetRandomUpgradePath(paths);
                if (att != null)
                    paths.Add(att);
            }

            if (paths.Count > 0)
            {
                Upgrades.Enqueue(new([.. paths]));
                Parent.SendHint("<color=#00FF00><b>Upgrades available!</b></color>\nPress \"~\" and type \".su\" (for more detail) to see available choices, type \".c <number>\" to choose an upgrade. \nOR bind a key in <b>Server Specific Settings</b>", [HintEffectPresets.FadeOut()], 15f);
                Parent.SendConsoleMessage(Peek(), "white");
            }
        }

        public string Peek(bool brief = false)
        {
            string unBrief = Peek(out string bri);
            return !brief ? unBrief : bri;
        }

        public string Peek(out string brief)
        {
            if (Upgrades.Count <= 0)
            {
                brief = "No upgrades available.";
                return brief;
            }

            Item t = Upgrades.Peek();
            StringBuilder sb = new("Choices: \n");
            StringBuilder br = new("Choices: \n");

            for (int i = 0; i < t.Choices.Length; i++)
            {
                sb.Append(i + 1);
                sb.Append(" - ");
                sb.Append(t.Choices[i].Perk.Profile.FancyName);
                sb.AppendLine();
                br.Append(i + 1);
                br.Append(" - ");
                br.Append(t.Choices[i].Perk.Profile.FancyName);
                br.AppendLine("");
                sb.AppendLine(t.Choices[i].Perk.Profile.Description);
                sb.AppendLine();
            }

            brief = br.ToString();
            return sb.ToString();
        }

        public bool Choose(int index, out string name)
        {
            if (Upgrades.Count <= 0)
            {
                name = "";
                return false;
            }

            Item t = Upgrades.Peek();

            if (index < 0 || index >= t.Choices.Length)
            {
                name = "";
                return false;
            }

            if (Parent.TryGetPerkInventory(out PerkInventory inv) && inv.TryGetPerk(t.Choices[index].Perk.Perk, out PerkBase perkBase) && perkBase is UpgradePathPerkBase b)
            {
                if (b.Maxed)
                {
                    name = "";
                    return false;
                }

                b.Progress++;
            }
            else
                Parent.GivePerk(t.Choices[index].Perk);

            Upgrades.Dequeue();

            name = t.Choices[index].Perk.Profile.FancyName;
            return true;
        }
    }
}
