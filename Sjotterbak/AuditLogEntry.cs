using System;
using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public class AuditLogEntry
    {
        [XmlAttribute]
        public DateTime Timestamp { get; set; }
        [XmlElement]
        public string Message { get; set; }
    }
}