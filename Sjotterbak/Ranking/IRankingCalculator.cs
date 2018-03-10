using System;
using System.Collections.Generic;
using System.Text;

namespace Sjotterbak.Ranking
{
    public interface IPlayerRankingCalculator
    {
        IEnumerable<PlayerRankingEntry> DetermineRanking(Records data);
        string Name { get; }
    }
}
