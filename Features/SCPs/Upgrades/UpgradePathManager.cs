using PlayerRoles;
using SwiftUHC.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SwiftUHC.Features.SCPs.Upgrades
{
    public static class UpgradePathManager
    {
        public static readonly Dictionary<string, UpgradePathAttribute> RegisteredUpgrades = [];

        public static void Enable()
        {
            RegisterUpgrades("base");
        }

        public static void Disable()
        {

        }

        public static void RegisterUpgrades(string nameSpace)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            Dictionary<Type, UpgradePathAttribute> atts = callingAssembly
                .GetTypes()
                .Select(t => (type: t, attr: t.GetCustomAttribute<UpgradePathAttribute>()))
                .Where(pair => pair.attr != null)
                .ToDictionary(pair => pair.type, pair => pair.attr);

            foreach (KeyValuePair<Type, UpgradePathAttribute> attr in atts)
            {
                attr.Value.Perk = PerkManager.GetPerk(attr.Key);
                RegisteredUpgrades.Add((RegisteredUpgrades.ContainsKey(attr.Value.ID) ? nameSpace.ToLower() + "." : "") + attr.Value.ID.ToLower(), attr.Value);
            }
        }

        public static UpgradePathAttribute GetRandomUpgradePath(this RoleTypeId role) => RegisteredUpgrades.Values.Where((t) => t.Role == role).ToArray().GetWeightedRandom();
        public static UpgradePathAttribute GetRandomUpgradePath(this RoleTypeId role, ICollection<UpgradePathAttribute> noRep) => RegisteredUpgrades.Values.Where((t) => t.Role == role && !noRep.Contains(t)).ToArray().GetWeightedRandom();
    }
}
