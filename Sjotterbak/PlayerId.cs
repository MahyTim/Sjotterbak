using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public struct PlayerId
    {
        [XmlAttribute]
        public int Value { get; set; }
    }

}
