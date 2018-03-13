using System.Collections.Generic;

namespace Sjotterbak.Ranking
{
    public interface IPlayerRankingCalculator
    {
        IEnumerable<PlayerRankingEntry> DetermineRanking(Records data);
        string Name { get; }
    }
}
