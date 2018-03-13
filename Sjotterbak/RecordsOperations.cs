using System;
using System.Collections.Generic;
using System.Linq;

namespace Sjotterbak
{
    public static class RecordsOperations
    {
        public static IEnumerable<Player> GetPlayers(this Records records)
        {
            return records.Games.SelectMany(z => z.GetPlayers()).Distinct();
        }

        public static Game AddGame(this Records records, Player team1Player1, Player team1Player2, Player team2Player1, Player team2Player2, int scoreTeam1, int scoreTeam2)
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
    }
}