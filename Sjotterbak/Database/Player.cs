using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sjotterbak
{
    [XmlType]
    public struct Player : IEquatable<Player>
    {
        [XmlElement]
        public string Name { get; set; }

        public Player(string name)
        {
            this.Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is Player && Equals((Player)obj);
        }

        public bool Equals(Player other)
        {
            return string.Equals(Name.Trim(), other.Name.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            var hashCode = 266367750;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name.Trim().ToLowerInvariant());
            return hashCode;
        }

        public static bool operator ==(Player player1, Player player2)
        {
            return player1.Equals(player2);
        }

        public static bool operator !=(Player player1, Player player2)
        {
            return !(player1 == player2);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
