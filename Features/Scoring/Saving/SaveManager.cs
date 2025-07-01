using LabApi.Features.Wrappers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SwiftArcadeMode.Features.Scoring.Saving
{
    public static class SaveManager
    {
        /// <summary>
        /// User ID (Steam ID) to score number.
        /// </summary>
        public static readonly Dictionary<string, int> Scores = [];

        public static string SavePath;

        public static Player IDToPlayer(string id) => Player.Get(id);

        public static void SaveScores()
        {
            StringBuilder stringBuilder = new();

            foreach (var s in Scores)
            {
                stringBuilder.Append(s.Key);
                stringBuilder.Append(';');
                stringBuilder.Append(s.Value);
                stringBuilder.Append('\n');
            }

            File.WriteAllText(SavePath, stringBuilder.ToString());
        }

        public static void LoadScores()
        {
            if (!File.Exists(SavePath))
                return;

            string[] str = File.ReadAllLines(SavePath);

            Scores.Clear();

            foreach (var s in str)
            {
                string[] split = s.Split(';');
                Scores.Add(split[0], int.Parse(split[1]));
            }
        }
    }
}
