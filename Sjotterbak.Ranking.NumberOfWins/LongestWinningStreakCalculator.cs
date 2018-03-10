using System.Collections.Generic;
using System.Linq;

namespace Sjotterbak.Ranking.EasyStats
{
    public class LongestWinningStreakCalculator : IPlayerRankingCalculator
    {
        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            foreach (var player in data.Players)
            {
                var games = data.Games.Where(z => z.IsPlayer(player));
                int longestStreak = 0;
                int currentStreak = 0;
                foreach (var game in games)
                {
                    if (game.IsWinner(player))
                    {
                        currentStreak++;
                    }
                    else
                    {
                        if (currentStreak > longestStreak)
                        {
                            longestStreak = currentStreak;
                        }
                        currentStreak = 0;
                    }
                }

                yield return new PlayerRankingEntry()
                {
                    PlayerId = player,
                    Score = longestStreak
                };
            }
        }

        public string Name => "Longest winning streak";
    }
}