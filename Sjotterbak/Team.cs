using System;
using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public struct Team
    {
        [XmlElement]
        public TeamId Id { get; set; }
        [XmlElement]
        public PlayerId PlayerOne { get; set; }
        [XmlElement]
        public PlayerId PlayerTwo { get; set; }
        [XmlAttribute]
        public DateTime CreatedTimestamp { get; set; }
    }

}
