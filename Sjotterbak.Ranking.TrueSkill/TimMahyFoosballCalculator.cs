using System;
using System.Collections.Generic;
using Moserware.Skills;
using Moserware.Skills.TrueSkill;

namespace Sjotterbak.Ranking.TrueSkill
{
    public class TimMahyFoosballCalculator : IPlayerRankingCalculator
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
                for (int i = 0; i < game.ScoreTeam1; i++)
                {
                    CalculatePoint(game,true);
                }
                for (int i = 0; i < game.ScoreTeam2; i++)
                {
                    CalculatePoint(game,false);
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

        private void CalculatePoint(Game game,bool team1Wins)
        {
            var team1 = new Moserware.Skills.Team()
                .AddPlayer(_players[game.Team1Player1], _ratings[game.Team1Player1])
                .AddPlayer(_players[game.Team1Player2], _ratings[game.Team1Player2]);

            var team2 = new Moserware.Skills.Team()
                .AddPlayer(_players[game.Team2Player1], _ratings[game.Team2Player1])
                .AddPlayer(_players[game.Team2Player2], _ratings[game.Team2Player2]);


            var teams = Moserware.Skills.Teams.Concat(team1, team2);
            var newRatingsWinLose = _calculator.CalculateNewRatings(_gameInfo, teams,
                team1Wins ? 1 : 2, team1Wins == false ? 1 : 2);

            foreach (var newRating in newRatingsWinLose)
            {
                _ratings[(Player) newRating.Key.Id] = newRating.Value;
            }
        }

        public string Name => "TimMahyFoosballCalculator";
    }
}