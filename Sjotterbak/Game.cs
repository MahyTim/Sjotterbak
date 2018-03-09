using System;

namespace Sjotterbak
{
    public struct Game
    {
        public GameId Id { get; set; }
        public TeamId Team1 { get; set; }
        public TeamId Team2 { get; set; }
        public int ScoreTeam1 { get; set; }
        public int ScoreTeam2 { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }

}
