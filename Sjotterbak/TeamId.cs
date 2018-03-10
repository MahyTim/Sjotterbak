using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public struct TeamId
    {
        [XmlAttribute]
        public int Value { get; set; }
    }

}
