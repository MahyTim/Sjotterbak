using System;
using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public struct Player
    {
        [XmlElement]
        public PlayerId Id { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlAttribute]
        public DateTime CreatedTimestamp { get; set; }
    }

}
