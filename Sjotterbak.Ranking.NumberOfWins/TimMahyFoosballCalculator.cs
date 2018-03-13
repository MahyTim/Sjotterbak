using System;
using System.Collections.Generic;

namespace Sjotterbak.Ranking.EasyStats
{
    public class TimMahyFoosballCalculator : IPlayerRankingCalculator
    {
        private readonly Dictionary<Player, double> _ratings = new Dictionary<Player, double>();

        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            var gamePoints = new Dictionary<int, double>()
            {
                {1, 1},
                {2, 1},
                {3, 2},
                {4, 3},
                {5, 4},
                {6, 4},
                {7, 4},
                {8, 5},
                {9, 5},
                {10, 7},
                {11, 11}
            };
            foreach (var player in data.GetPlayers())
            {
                _ratings[player] = 4;
            }

            foreach (var game in data.Games)
            {
                var scoreDifferential = Math.Abs(game.ScoreTeam1 - game.ScoreTeam2);
                var scoreAdjustement = gamePoints[scoreDifferential];

                var isOvertime = Math.Max(game.ScoreTeam1, game.ScoreTeam2) > 11;
                if (isOvertime)
                {
                    var factorOvertime = 0.3;
                    scoreAdjustement = scoreAdjustement + (Math.Abs(scoreAdjustement) * factorOvertime);
                }

                foreach (var player in game.GetPlayers())
                {
                    this._ratings[player] += game.IsWinner(player) ? scoreAdjustement : scoreAdjustement * -1;
                }
            }

            foreach (var rating in _ratings)
            {
                yield return new PlayerRankingEntry()
                {
                    Player = rating.Key,
                    Score = rating.Value
                };
            }
        }

        public string Name => "TimMahyFoosballCalculator";
    }
}