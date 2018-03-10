using System;
using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public struct Game
    {
        [XmlElement]
        public GameId Id { get; set; }
        [XmlElement]
        public TeamId Team1 { get; set; }
        [XmlElement]
        public TeamId Team2 { get; set; }
        [XmlAttribute]
        public int ScoreTeam1 { get; set; }
        [XmlAttribute]
        public int ScoreTeam2 { get; set; }
        [XmlAttribute]
        public DateTime CreatedTimestamp { get; set; }
    }

}
