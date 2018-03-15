using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sjotterbak.WebApi.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sjotterbak.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Stats")]
    [EnableCors("AllowAll")]
    public class StatsController : Controller
    {
        public class Stats
        {
            public PlayingFieldStats PlayingField { get; set; }
            public PlayerStats[] PlayerStats { get; set; }
            public TeamStats[] TeamStats { get; set; }
        }

        public class PlayingFieldStats
        {
            public int NumberOfGames { get; set; }
            public ColorStats Blue { get; set; }
            public ColorStats Red { get; set; }
        }

        public class ColorStats
        {
            public int NumberOfWins { get; set; }
            public double WinPercentage { get; set; }
        }

        public class PlayerStats
        {
            public string Player { get; set; }
            public int NumberOfWins { get; set; }
            public int NumberOfLosses { get; set; }
            public int NumberOfGames { get; set; }
            public double WinPercentage { get; set; }
            public int NumberOfGamesAsAttacker { get; set; }
            public int NumberOfGamesAsKeeper { get; set; }
            public int LongestWinningStreak { get; set; }
            public int NumberOfWinsithBlue { get; set; }
            public int NumberOfWinsWithRed { get; set; }
            public string BestPartnerPlayer { get; set; }
        }

        public class TeamStats
        {
            public string[] Players { get; set; }
            public int NumberOfWins { get; set; }
            public int NumberOfLosses { get; set; }
            public int NumberOfGames { get; set; }
            public double WinPercentage { get; set; }
        }

        private readonly DatabaseService _service;

        public StatsController(DatabaseService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Stats))]
        public Stats Get()
        {
            return new Stats()
            {
                PlayingField = GetPlayingFieldStats(),
                PlayerStats = GetPlayerStats(),
                TeamStats = GetTeamStats()
            };
        }

        [HttpGet("/TeamStats")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TeamStats[]))]
        public TeamStats[] GetTeamStats()
        {
            return new[] { new TeamStats() };
        }

        [HttpGet("/PlayerStats")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PlayerStats[]))]
        public PlayerStats[] GetPlayerStats()
        {
            return new[] { new PlayerStats() };
        }

        [HttpGet("/PlayerStats/{name}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PlayerStats))]
        public PlayerStats GetPlayerStats(string name)
        {
            return new PlayerStats();
        }

        [HttpGet("/PlayingField")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PlayingFieldStats))]
        public PlayingFieldStats GetPlayingFieldStats()
        {
            var stats = new PlayingFieldStats()
            {
                Blue = new ColorStats(),
                Red = new ColorStats(),
            };

            foreach (var game in _service.Records().Games)
            {
                stats.NumberOfGames++;
                if (game.ScoreTeam1 > game.ScoreTeam2)
                {
                    stats.Blue.NumberOfWins++;
                }
                else
                {
                    stats.Red.NumberOfWins++;
                }
            }
            stats.Blue.WinPercentage = stats.Blue.NumberOfWins / stats.NumberOfGames * 100;
            stats.Red.WinPercentage = stats.Red.NumberOfWins / stats.NumberOfGames * 100;
            return stats;
        }
    }
}
