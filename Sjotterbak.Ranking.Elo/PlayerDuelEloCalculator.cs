using System;
using System.Collections.Generic;
using Moserware.Skills;
using Moserware.Skills.Elo;
using Moserware.Skills.TrueSkill;

namespace Sjotterbak.Ranking.Elo
{
    public class PlayerDuelEloFideCalculator : IPlayerRankingCalculator
    {
        private readonly GameInfo _gameInfo = GameInfo.DefaultGameInfo;
        private readonly SkillCalculator _calculator = new DuellingEloCalculator(new FideEloCalculator());
        private readonly Dictionary<Moserware.Skills.Player, Rating> _ratings = new Dictionary<Moserware.Skills.Player, Rating>();
        private readonly Dictionary<PlayerId, Moserware.Skills.Player> _players = new Dictionary<PlayerId, Moserware.Skills.Player>();

        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            foreach (var player in data.Players)
            {
                _players[player.Id] = new Moserware.Skills.Player(player.Id);
                _ratings[_players[player.Id]] = GameInfo.DefaultGameInfo.DefaultRating;
            }

            foreach (var game in data.Games)
            {
                var team1Player1 = _players[data.Expand(game.Team1).PlayerOne];
                var team1Player2 = _players[data.Expand(game.Team1).PlayerTwo];
                var team2Player1 = _players[data.Expand(game.Team2).PlayerOne];
                var team2Player2 = _players[data.Expand(game.Team2).PlayerTwo];

                var team1 = new Moserware.Skills.Team()
                    .AddPlayer(team1Player1, _ratings[team1Player1])
                    .AddPlayer(team1Player2, _ratings[team1Player2]);

                var team2 = new Moserware.Skills.Team()
                    .AddPlayer(team2Player1, _ratings[team2Player1])
                    .AddPlayer(team2Player2, _ratings[team2Player2]);

                var teams = Moserware.Skills.Teams.Concat(team1, team2);
                var newRatingsWinLose = _calculator.CalculateNewRatings(_gameInfo, teams, game.ScoreTeam1 > game.ScoreTeam2 ? 1 : 2, game.ScoreTeam2 > game.ScoreTeam1 ? 1 : 2);

                foreach (var newRating in newRatingsWinLose)
                {
                    _ratings[newRating.Key] = newRating.Value;
                }
            }

            foreach (var rating in _ratings)
            {
                yield return new PlayerRankingEntry()
                {
                    PlayerId = (PlayerId)rating.Key.Id,
                    Score = rating.Value.Mean
                };
            }
        }

        public string Name => "DuelElo - Fide";
    }
}
