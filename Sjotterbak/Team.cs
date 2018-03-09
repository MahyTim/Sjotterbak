using System;

namespace Sjotterbak
{
    public struct Team
    {
        public TeamId Id { get; set; }
        public PlayerId PlayerOne { get; set; }
        public PlayerId PlayerTwo { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }

}
