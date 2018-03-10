using System.Collections.Generic;

namespace Sjotterbak.Ranking.EasyStats
{
    public class WinsPercentageCalculator : IPlayerRankingCalculator
    {
        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            yield break;
        }

        public string Name => "Win %";
    }
}
