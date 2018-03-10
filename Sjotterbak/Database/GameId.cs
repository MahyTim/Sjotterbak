using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public struct GameId
    {
        [XmlAttribute]
        public int Value { get; set; }
    }

}
