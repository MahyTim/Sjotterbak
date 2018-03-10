using System.Collections.Generic;
using System.Linq;

namespace Sjotterbak.Ranking.EasyStats
{
    public class WinsPercentageCalculator : IPlayerRankingCalculator
    {
        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            foreach (var player in data.Players)
            {
                yield return new PlayerRankingEntry()
                {
                    PlayerId = player.Id,
                    Score = (double)data.Games.Count(z => z.IsWinner(player.Id)) / (double)data.Games.Count(z => z.IsPlayer(player.Id)) * 100
                };
            }
        }

        public string Name => "Win %";
    }
}
