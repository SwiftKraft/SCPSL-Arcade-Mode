using LabApi.Features.Wrappers;
using System.Collections.Generic;

namespace SwiftArcadeMode.Features.Scoring
{
    public static class ScoringManager
    {
        /// <summary>
        /// User ID (Steam ID) to score number.
        /// </summary>
        public static readonly Dictionary<string, int> Scores = [];

        public static void Enabled()
        {

        }

        public static void AddScore(this Player p, int amount)
        {
            if (Scores.ContainsKey(p.UserId))
                Scores[p.UserId] += amount;
            else
                Scores.Add(p.UserId, amount);
        }
    }
}
