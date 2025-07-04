﻿using Hints;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Commands.Client;
using SwiftArcadeMode.Features;
using System;
using UnityEngine;
using UserSettings.ServerSpecific;
using Logger = LabApi.Features.Console.Logger;

#if EXILED
using Exiled.API.Features.Core.UserSettings;
#endif

namespace SwiftArcadeMode.ServerSpecificSettings
{
    public static class SSSManager
    {
        public static SSKeybindSetting Keybind_SeePerks;
        public static SSKeybindSetting Keybind_SeeUpgrades;
        public static SSKeybindSetting Keybind_ChooseUpgrade1;
        public static SSKeybindSetting Keybind_ChooseUpgrade2;
        public static SSKeybindSetting Keybind_ChooseUpgrade3;

        public static void Enable()
        {
            Keybind_SeePerks = new(20070923, "See Perks", KeyCode.LeftBracket, true, true, "Shows you your perks.");
            Keybind_SeeUpgrades = new(20070924, "See Upgrades", KeyCode.RightBracket, true, false, "Shows you your available upgrades.");
            Keybind_ChooseUpgrade1 = new(20070925, "Choose Upgrade 1", KeyCode.Comma, true, false, "Chooses upgrade 1.");
            Keybind_ChooseUpgrade2 = new(20070926, "Choose Upgrade 2", KeyCode.Period, true, false, "Chooses upgrade 2.");
            Keybind_ChooseUpgrade3 = new(20070927, "Choose Upgrade 3", KeyCode.Slash, true, false, "Chooses upgrade 3.");

            AppendSettings();
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnPressKeybind;
            PlayerEvents.Joined += OnJoined;
        }

        private static void AppendSettings()
        {
#if LAB_API
            ServerSpecificSettingsSync.DefinedSettings = [.. ServerSpecificSettingsSync.DefinedSettings ?? [], new SSGroupHeader("SwiftKraft's Arcade Mode"), Keybind_SeePerks, Keybind_SeeUpgrades, Keybind_ChooseUpgrade1, Keybind_ChooseUpgrade2, Keybind_ChooseUpgrade3];
            ServerSpecificSettingsSync.SendToAll();
#elif EXILED
            Logger.Info("Loading EXILED SSS...");
            SettingBase.Register([SettingBase.Create(new SSGroupHeader("SwiftKraft's Arcade Mode")), SettingBase.Create(Keybind_SeePerks), SettingBase.Create(Keybind_SeeUpgrades), SettingBase.Create(Keybind_ChooseUpgrade1), SettingBase.Create(Keybind_ChooseUpgrade2), SettingBase.Create(Keybind_ChooseUpgrade3)]);
            SettingBase.SendToAll();
#endif
        }

        private static void OnJoined(PlayerJoinedEventArgs ev)
        {
#if LAB_API
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
#elif EXILED
            Logger.Info("Sending EXILED SSS to player: " + ev.Player);
            SettingBase.SendToPlayer(ev.Player);
#endif
        }

        private static void OnPressKeybind(ReferenceHub pl, ServerSpecificSettingBase setting)
        {
            try
            {
                Player p = Player.Get(pl);

                if (p == null)
                    return;

                PerkInventory inv = p.GetPerkInventory();

                void select(int ind)
                {
                    if (inv == null)
                    {
                        p.SendHint("<align=\"left\">No upgrades available.</align>", [HintEffectPresets.FadeOut()], 2f);
                        return;
                    }

                    if (inv.UpgradeQueue.Choose(ind, out string name))
                        p.SendHint("<align=\"left\">Chosen: " + name + "</align>", [HintEffectPresets.FadeOut()], 10f);
                    else
                        p.SendHint("<align=\"left\">Failed to choose upgrade.</align>", [HintEffectPresets.FadeOut()], 2f);
                }

                switch (setting)
                {
                    case SSKeybindSetting { SettingId: 20070923, SyncIsPressed: true } keyb: // see perks
                        ClientSeePerksCommand.GetPerks(p, out string brief1);
                        p.SendHint(brief1, [HintEffectPresets.FadeOut()], 5f);
                        break;
                    case SSKeybindSetting { SettingId: 20070924, SyncIsPressed: true } keyb: // see upgrades
                        if (inv == null)
                        {
                            p.SendHint("<align=\"left\">No upgrades available.</align>", [HintEffectPresets.FadeOut()], 2f);
                            break;
                        }
                        string text = inv.UpgradeQueue.Peek(out string n);
                        p.SendHint("<align=\"left\">" + n + "</align>", [HintEffectPresets.FadeOut()], 10f);
                        break;
                    case SSKeybindSetting { SettingId: 20070925, SyncIsPressed: true } keyb: // choose 1
                        select(0);
                        break;
                    case SSKeybindSetting { SettingId: 20070926, SyncIsPressed: true } keyb: // choose 2
                        select(1);
                        break;
                    case SSKeybindSetting { SettingId: 20070927, SyncIsPressed: true } keyb: // choose 3
                        select(2);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Tick()
        {

        }

        public static void Disable() => PlayerEvents.Joined -= OnJoined;
    }
}
