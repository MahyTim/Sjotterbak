using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public struct PlayerId
    {
        [XmlAttribute]
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is PlayerId))
            {
                return false;
            }

            var id = (PlayerId)obj;
            return Value == id.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = -783812246;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }
    }

}
