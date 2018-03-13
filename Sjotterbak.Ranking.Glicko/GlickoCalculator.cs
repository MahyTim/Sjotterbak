using System.Collections.Generic;
using Glicko2;

namespace Sjotterbak.Ranking.Glicko
{
    public class GlickoCalculator : Sjotterbak.Ranking.IPlayerRankingCalculator
    {
        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            var calculator = new RatingCalculator();

            var players = new Dictionary<PlayerId, Rating>();
            foreach (var player in data.Players)
            {
                players[player] = new Rating(calculator);
            }
            var results = new RatingPeriodResults();

            foreach (var game in data.Games)
            {
                foreach (var winner in game.Winners())
                {
                    foreach (var loser in game.Losers())
                    {
                        results.AddResult(players[winner], players[loser]);
                    }
                }
            }
            calculator.UpdateRatings(results);

            foreach (var player in players)
            {
                yield return new PlayerRankingEntry()
                {
                    PlayerId = player.Key,
                    Score = player.Value.GetRating()
                };
            }
        }

        public string Name => "Glicko";
    }
}
