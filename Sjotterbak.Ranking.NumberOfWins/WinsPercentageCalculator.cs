using System.Collections.Generic;

namespace Sjotterbak.Ranking.EasyStats
{
    public class WinsPercentageCalculator : IPlayerRankingCalculator
    {
        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            var winsPerPlayer = new Dictionary<PlayerId, int>();
            foreach (var game in data.Games)
            {
                var winningTeam = game.ScoreTeam1 > game.ScoreTeam2 ? game.Team1 : game.Team2;
                var winningPlayers = new PlayerId[]
                    {data.Expand(winningTeam).PlayerOne, data.Expand(winningTeam).PlayerTwo};
                foreach (var winningPlayer in winningPlayers)
                {
                    winsPerPlayer[winningPlayer] = winsPerPlayer.ContainsKey(winningPlayer) == false
                        ? 1
                        : winsPerPlayer[winningPlayer] + 1;
                }
            }

            foreach (var i in winsPerPlayer)
            {
                yield return new PlayerRankingEntry()
                {
                    PlayerId = i.Key,
                    Score = i.Value / data.Games.Count * 100
                };
            }
        }

        public string Name => "Win %";
    }
}
