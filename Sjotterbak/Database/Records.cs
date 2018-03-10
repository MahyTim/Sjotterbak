using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlRoot]
    public class Records
    {
        [XmlArray]
        public List<Game> Games = new List<Game>();
        [XmlArray]
        public List<Player> Players = new List<Player>();
        [XmlArray]
        public List<Team> Teams = new List<Team>();
        [XmlElement]
        public MetaData MetaData = new MetaData();
        [XmlArray]
        public List<AuditLogEntry> AuditLogEntries = new List<AuditLogEntry>();
    }
}
