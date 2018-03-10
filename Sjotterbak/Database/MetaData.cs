using System;
using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public class MetaData
    {
        [XmlAttribute]
        public DateTime CreatedTimestamp { get; set; }
    }
}