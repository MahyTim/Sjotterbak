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
        public PlayerId Team1Player1 { get; set; }
        [XmlElement]
        public PlayerId Team1Player2 { get; set; }
        [XmlElement]
        public PlayerId Team2Player1 { get; set; }
        [XmlElement]
        public PlayerId Team2Player2 { get; set; }
        [XmlAttribute]
        public int ScoreTeam1 { get; set; }
        [XmlAttribute]
        public int ScoreTeam2 { get; set; }
        [XmlAttribute]
        public DateTime CreatedTimestamp { get; set; }
    }

    public static class GameHelper
    {
        public static bool IsPlayer(this Game game, PlayerId player)
        {
            return game.Team1Player1 == player
                   || game.Team1Player2 == player
                   || game.Team2Player1 == player
                   || game.Team2Player2 == player;
        }

        public static bool IsWinner(this Game game, PlayerId player)
        {
            return Winners(game).Contains(player);
        }

        public static IEnumerable<PlayerId> Winners(this Game game)
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
