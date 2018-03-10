using System;
using System.Linq;

namespace Sjotterbak
{
    public static class RecordsOperations
    {
        public static Game AddGame(this Records records, TeamId team1, TeamId team2, int scoreTeam1, int scoreTeam2)
        {
            var game = new Game()
            {
                Id = new GameId()
                {
                    Value = records.Games.Any() == false ? 0 : records.Games.Max(z => z.Id.Value) + 1
                },
                ScoreTeam1 = scoreTeam1,
                ScoreTeam2 = scoreTeam2,
                Team1 = team1,
                Team2 = team2,
                CreatedTimestamp = DateTime.UtcNow
            };
            records.Games.Add(game);
            return game;
        }

        public static TeamId GetOrCreateTeam(this Records records, string player1, string player2)
        {
            var player1Id = records.Players.First(z => z == new Player() { Name = player1 }).Id;
            var player2Id = records.Players.First(z => z == new Player() { Name = player2 }).Id;

            var teamExists = records.Teams.Any(z => z.PlayerOne.Equals(player1Id) && z.PlayerTwo.Equals(player2Id));
            if (teamExists)
            {
                return records.Teams.First(z => z.PlayerOne.Equals(player1Id) && z.PlayerTwo.Equals(player2Id)).Id;
            }
            else
            {
                var team = new Team()
                {
                    Id = new TeamId()
                    {
                        Value = records.Teams.Any() == false ? 0 : records.Teams.Max(z => z.Id.Value) + 1
                    },
                    PlayerOne = player1Id,
                    PlayerTwo = player2Id,
                    CreatedTimestamp = DateTime.UtcNow
                };
                records.Teams.Add(team);
                return team.Id;
            }
        }

        public static Team Expand(this Records records, TeamId id)
        {
            return records.Teams.First(z => z.Id.Equals(id));
        }

        public static Player Expand(this Records records, PlayerId id)
        {
            return records.Players.First(z => z.Id.Equals(id));
        }
        public static Player AddPlayer(this Records records, string name)
        {
            var player = new Player()
            {
                Id = new PlayerId()
                {
                    Value = records.Players.Any() == false ? 0 : records.Players.Max(z => z.Id.Value) + 1
                },
                Name = name,
                CreatedTimestamp = DateTime.UtcNow
            };
            records.Players.Add(player);
            return player;
        }

        public static bool PlayerExists(this Records records, string name)
        {
            return records.Players.Any(z =>
                string.Equals(z.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}