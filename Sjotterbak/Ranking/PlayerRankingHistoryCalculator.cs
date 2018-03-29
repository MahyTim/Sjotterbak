using System;
using System.Collections.Generic;
using System.Linq;

namespace Sjotterbak.Ranking
{
    public class PlayerRankingHistoryCalculator
    {
        private readonly IPlayerRankingCalculator _algorithm;
        private readonly Records _records;

        public PlayerRankingHistoryCalculator(IPlayerRankingCalculator algorithm, Records records)
        {
            _algorithm = algorithm;
            _records = records;
        }

        public double[] GetScoringByPlayer(Player player)
        {
            var history = new List<double>();
            for (int i = 0; i < _records.Games.Count; i++)
            {
                var subsetOfGames = new Records()
                {
                    Games = _records.Games.Take(i).ToList()
                };
                if (subsetOfGames.Games.Any(z => z.IsPlayer(player)))
                {
                    var currentRanking = _algorithm.DetermineRanking(subsetOfGames).FirstOrDefault(z => z.Player == player);
                    if (currentRanking != null && (history.Any() == false || Math.Abs(history.Last() - currentRanking.Score) > 0.01))
                    {
                        history.Add(currentRanking.Score);
                    }
                }
            }
            return history.ToArray();
        }
    }
}