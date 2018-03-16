using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public struct Game
    {
        [XmlElement]
        public GameId Id { get; set; }
        [XmlElement]
        public Player Team1Player1 { get; set; }
        [XmlElement]
        public Player Team1Player2 { get; set; }
        [XmlElement]
        public Player Team2Player1 { get; set; }
        [XmlElement]
        public Player Team2Player2 { get; set; }
        [XmlAttribute]
        public int ScoreTeam1 { get; set; }
        [XmlAttribute]
        public int ScoreTeam2 { get; set; }
        [XmlAttribute]
        public DateTime CreatedTimestamp { get; set; }
    }

    public static class GameHelper
    {
        public static IEnumerable<Player> GetPlayers(this Game game)
        {
            yield return game.Team1Player1;
            yield return game.Team1Player2;
            yield return game.Team2Player1;
            yield return game.Team2Player2;
        }

        public static Player Partner(this Game game, Player player)
        {
            if (IsTeam1Player(game, player))
            {
                return game.Team1Player1 == player ? game.Team1Player2 : game.Team1Player1;
            }
            return game.Team2Player1 == player ? game.Team2Player2 : game.Team1Player1;
        }

        public static bool IsTeam1Player(this Game game, Player player)
        {
            return game.Team1Player1 == player || game.Team1Player2 == player;
        }

        public static bool IsTeam2Player(this Game game, Player player)
        {
            return game.Team2Player1 == player || game.Team2Player2 == player;
        }

        public static bool IsAttacker(this Game game, Player player)
        {
            return game.Team1Player2 == player || game.Team2Player2 == player;
        }
        public static bool IsKeeper(this Game game, Player player)
        {
            return game.Team1Player1 == player || game.Team2Player1 == player;
        }

        public static bool IsPlayer(this Game game, Player player)
        {
            return IsAttacker(game, player) || IsKeeper(game, player);
        }

        public static bool IsWinner(this Game game, Player player)
        {
            return Winners(game).Contains(player);
        }

        public static IEnumerable<Player> Losers(this Game game)
        {
            if (game.ScoreTeam1 < game.ScoreTeam2)
            {
                yield return game.Team1Player1;
                yield return game.Team1Player2;
            }
            else
            {
                yield return game.Team2Player1;
                yield return game.Team2Player2;
            }
        }
        public static IEnumerable<Player> Winners(this Game game)
        {
            if (game.ScoreTeam1 > game.ScoreTeam2)
            {
                yield return game.Team1Player1;
                yield return game.Team1Player2;
            }
            else
            {
                yield return game.Team2Player1;
                yield return game.Team2Player2;
            }
        }
    }
}
