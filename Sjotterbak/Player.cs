using System;

namespace Sjotterbak
{

    public struct Player
    {
        public PlayerId Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }

}
