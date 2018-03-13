using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlRoot]
    public class Records
    {
        [XmlArray]
        public List<Game> Games = new List<Game>();
        [XmlElement]
        public MetaData MetaData = new MetaData();
        [XmlArray]
        public List<AuditLogEntry> AuditLogEntries = new List<AuditLogEntry>();
    }
}
