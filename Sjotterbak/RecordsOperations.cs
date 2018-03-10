using System;
using System.Linq;

namespace Sjotterbak
{
    public static class RecordsOperations
    {
        public static Game AddGame(this Records records, PlayerId team1Player1, PlayerId team1Player2, PlayerId team2Player1, PlayerId team2Player2, int scoreTeam1, int scoreTeam2)
        {
            var game = new Game()
            {
                Id = new GameId()
                {
                    Value = records.Games.Any() == false ? 0 : records.Games.Max(z => z.Id.Value) + 1
                },
                ScoreTeam1 = scoreTeam1,
                ScoreTeam2 = scoreTeam2,
                Team1Player1 = team1Player1,
                Team1Player2 = team1Player2,
                Team2Player1 = team2Player1,
                Team2Player2 = team2Player2,
                CreatedTimestamp = DateTime.UtcNow
            };
            records.Games.Add(game);
            return game;
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

        public static PlayerId GetPlayer(this Records records, string name)
        {
            return records.Players.First(z =>
                string.Equals(z.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase)).Id;
        }
    }
}