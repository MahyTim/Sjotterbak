using System;
using System.Linq;

namespace Sjotterbak
{
    public static class RecordsOperations
    {
        public static Player AddPlayer(this Records records, string name)
        {
            var player = new Player()
            {
                Id = new PlayerId()
                {
                    Value = records.Players.Any() == false ? 0 : records.Players.Max(z => z.Id.Value) + 1
                },
                Name = name,
                CreatedTimestamp = DateTime.Now
            };
            records.Players.Add(player);
            return player;
        }

        public static bool PlayerExists(this Records records, string name)
        {
            return records.Players.Any(z =>
                string.Equals(z.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}