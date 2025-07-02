﻿using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using System.IO;
using System.Text;

namespace SwiftArcadeMode.Features.Scoring.Saving
{
    public static class SaveManager
    {
        public static string SavePath;
        public static string SaveFileName;
        public static string SaveDirectory;

        public static Player IDToPlayer(string id) => Player.Get(id);

        public static void SaveScores()
        {
            StringBuilder stringBuilder = new();

            foreach (var s in ScoringManager.Scores)
            {
                stringBuilder.Append(s.Key);
                stringBuilder.Append(';');
                stringBuilder.Append(s.Value);
                stringBuilder.Append(';');
                stringBuilder.Append(ScoringManager.IDToName[s.Key]);
                stringBuilder.Append('\n');
            }

            try
            {
                if (!Directory.Exists(SaveDirectory))
                    Directory.CreateDirectory(SaveDirectory);

                File.WriteAllText(SavePath, stringBuilder.ToString());
            }
            catch { }
            Logger.Info("Saved all scores!");
        }

        public static void LoadScores()
        {
            if (!File.Exists(SavePath))
                return;

            string[] str = File.ReadAllLines(SavePath);

            try
            {
                ScoringManager.Scores.Clear();

                foreach (var s in str)
                {
                    string[] split = s.Split(';');
                    ScoringManager.Scores.Add(split[0], int.Parse(split[1]));
                    ScoringManager.IDToName.Add(split[0], split[2]);
                }
            }
            catch { }
            Logger.Info("Loaded all scores!");
        }
    }
}
