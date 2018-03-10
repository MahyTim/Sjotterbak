using System.Collections.Generic;
using Moserware.Skills;
using Moserware.Skills.Elo;

namespace Sjotterbak.Ranking.Elo
{
    public class PlayerDuelEloGaussianCalculator : IPlayerRankingCalculator
    {
        private readonly GameInfo _gameInfo = GameInfo.DefaultGameInfo;
        private readonly SkillCalculator _calculator = new DuellingEloCalculator(new GaussianEloCalculator());
        private readonly Dictionary<PlayerId, Rating> _ratings = new Dictionary<PlayerId, Rating>();
        private readonly Dictionary<PlayerId, Moserware.Skills.Player> _players = new Dictionary<PlayerId, Moserware.Skills.Player>();

        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            foreach (var player in data.Players)
            {
                _players[player.Id] = new Moserware.Skills.Player(player.Id);
                _ratings[player.Id] = GameInfo.DefaultGameInfo.DefaultRating;
            }

            foreach (var game in data.Games)
            {

                var team1 = new Moserware.Skills.Team()
                    .AddPlayer(_players[game.Team1Player1], _ratings[game.Team1Player1])
                    .AddPlayer(_players[game.Team1Player2], _ratings[game.Team1Player2]);

                var team2 = new Moserware.Skills.Team()
                    .AddPlayer(_players[game.Team2Player1], _ratings[game.Team2Player1])
                    .AddPlayer(_players[game.Team2Player2], _ratings[game.Team2Player2]);

                var teams = Moserware.Skills.Teams.Concat(team1, team2);
                var newRatingsWinLose = _calculator.CalculateNewRatings(_gameInfo, teams, game.ScoreTeam1 > game.ScoreTeam2 ? 1 : 2, game.ScoreTeam2 > game.ScoreTeam1 ? 1 : 2);

                foreach (var newRating in newRatingsWinLose)
                {
                    _ratings[(PlayerId)newRating.Key.Id] = newRating.Value;
                }
            }

            foreach (var rating in _ratings)
            {
                yield return new PlayerRankingEntry()
                {
                    PlayerId = rating.Key,
                    Score = rating.Value.Mean
                };
            }
        }

        public string Name => "DuelElo - Gaussian";
    }
}