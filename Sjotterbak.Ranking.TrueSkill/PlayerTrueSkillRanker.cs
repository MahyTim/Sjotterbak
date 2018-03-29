using System.Collections.Generic;
using Moserware.Skills;
using Moserware.Skills.TrueSkill;

namespace Sjotterbak.Ranking.TrueSkill
{

    public class PlayerTrueSkillCalculator : IPlayerRankingCalculator
    {
        private readonly GameInfo _gameInfo = GameInfo.DefaultGameInfo;
        private readonly SkillCalculator _calculator = new TwoTeamTrueSkillCalculator();
        private readonly Dictionary<Player, Rating> _ratings = new Dictionary<Player, Rating>();
        private readonly Dictionary<Player, Moserware.Skills.Player> _players = new Dictionary<Player, Moserware.Skills.Player>();

        public IEnumerable<PlayerRankingEntry> DetermineRanking(Records data)
        {
            foreach (var player in data.GetPlayers())
            {
                _players[player] = new Moserware.Skills.Player(player);
                _ratings[player] = GameInfo.DefaultGameInfo.DefaultRating;
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
                    _ratings[(Player)newRating.Key.Id] = newRating.Value;
                }
            }

            foreach (var rating in _ratings)
            {
                yield return new PlayerRankingEntry()
                {
                    Player = rating.Key,
                    Score = rating.Value.Mean
                };
            }
        }

        public string Name => "TrueSkill";
    }
}
