using System;
using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public struct PlayerId : IEquatable<PlayerId>
    {
        [XmlAttribute]
        public int Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PlayerId && Equals((PlayerId)obj);
        }

        public bool Equals(PlayerId other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = -783812246;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(PlayerId id1, PlayerId id2)
        {
            return id1.Equals(id2);
        }

        public static bool operator !=(PlayerId id1, PlayerId id2)
        {
            return !(id1 == id2);
        }

       
    }

}
