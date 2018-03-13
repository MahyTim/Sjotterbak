using System.Collections.Generic;
using System.Linq;

namespace Sjotterbak.Ranking.EasyStats
{
    public class WinsPercentageCalculator : IPlayerRankingCalculator
    {
        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            foreach (var player in data.GetPlayers())
            {
                yield return new PlayerRankingEntry()
                {
                    Player = player,
                    Score = (double)data.Games.Count(z => z.IsWinner(player)) / (double)data.Games.Count(z => z.IsPlayer(player)) * 100
                };
            }
        }

        public string Name => "Win %";
    }
}
