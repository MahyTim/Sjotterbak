using System.Collections.Generic;
using System.Linq;

namespace Sjotterbak.Ranking.EasyStats
{
    public class LongestWinningStreakCalculator : IPlayerRankingCalculator
    {
        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            foreach (var player in data.GetPlayers())
            {
                int longestStreak = 0;

                var games = data.Games.Where(z => z.IsPlayer(player));
                int currentStreak = 0;
                foreach (var game in games)
                {
                    if (game.IsWinner(player))
                    {
                        currentStreak++;
                    }
                    else
                    {
                        currentStreak = 0;
                    }
                    if (currentStreak > longestStreak)
                    {
                        longestStreak = currentStreak;
                    }
                }

                yield return new PlayerRankingEntry()
                {
                    Player = player,
                    Score = longestStreak
                };
            }
        }

        public string Name => "Longest winning streak";
    }
}