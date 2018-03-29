using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Moserware.Skills;
using Moserware.Skills.TrueSkill;
using Sjotterbak.Ranking.EasyStats;
using Sjotterbak.Ranking.TrueSkill;
using Sjotterbak.WebApi.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sjotterbak.WebApi.Controllers
{
    [Produces("application/json")]
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
            public PlayersController.Player Player { get; set; }
            public int NumberOfWins { get; set; }
            public int NumberOfLosses { get; set; }
            public int NumberOfGames { get; set; }
            public double WinPercentage
            {
                get
                {
                    return (double)NumberOfWins / (double)NumberOfGames * 100;
                }
            }
            public int NumberOfGamesAsAttacker { get; set; }
            public int NumberOfGamesAsKeeper { get; set; }
            public int LongestWinningStreak { get; set; }
            public int NumberOfWinsWithBlue { get; set; }
            public int NumberOfWinsWithRed { get; set; }
            public PlayersController.Player BestPartnerPlayer { get; set; }
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

        [HttpGet("api/Stats")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Stats))]
        public Stats Get()
        {
            return new Stats()
            {
                PlayingField = GetPlayingFieldStats(),
                PlayerStats = DeterminePlayerStats(),
                TeamStats = GetTeamStats()
            };
        }

        [HttpGet("api/Stats/TeamStats")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TeamStats[]))]
        public TeamStats[] GetTeamStats()
        {
            return new[] { new TeamStats() };
        }

        [HttpGet("api/Stats/PlayerStats")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PlayerStats[]))]
        public IActionResult GetPlayerStats()
        {
            try
            {
                var stats = DeterminePlayerStats();
                return Json(stats);
            }
            catch (Exception e)
            {
                return base.StatusCode(500, e.Message);
            }
        }

        private PlayerStats[] DeterminePlayerStats()
        {
            var winningStreakRankingCalculator = new LongestWinningStreakCalculator();
            var winningStreakRanking = winningStreakRankingCalculator.DetermineRanking(_service.Records());

            var stats = new List<PlayerStats>();
            foreach (var player in _service.Records().GetPlayers())
            {
                var playerStats = new PlayerStats()
                {
                    Player = new PlayersController.Player(player),
                    NumberOfGames = _service.Records().Games.Count(z => z.IsPlayer(player)),
                    LongestWinningStreak = (int)(winningStreakRanking.Any(z => z.Player == player) == false ? 0 : winningStreakRanking.FirstOrDefault(z => z.Player == player).Score),
                    NumberOfGamesAsAttacker = _service.Records().Games.Count(z => z.IsAttacker(player)),
                    NumberOfGamesAsKeeper = _service.Records().Games.Count(z => z.IsKeeper(player)),
                    NumberOfLosses = _service.Records().Games.Where(z => z.IsPlayer(player)).Count(z => z.IsWinner(player) == false),
                    NumberOfWins = _service.Records().Games.Where(z => z.IsPlayer(player)).Count(z => z.IsWinner(player) == true),
                    NumberOfWinsWithBlue = _service.Records().Games.Where(z => z.IsPlayer(player)).Where(z => z.IsWinner(player)).Count(z => z.IsTeam1Player(player)),
                    NumberOfWinsWithRed = _service.Records().Games.Where(z => z.IsPlayer(player)).Where(z => z.IsWinner(player)).Count(z => z.IsTeam2Player(player)),
                    BestPartnerPlayer = new PlayersController.Player(DetermineBestPartner(player.Name)),
                };
                stats.Add(playerStats);
            }
            return stats.ToArray();
        }

        [HttpGet("api/Stats/PlayerStats/BestPartner/{name}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PlayersController.Player))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult GetBestPartner(string name)
        {
            Player bestPartner = DetermineBestPartner(name);
            if (bestPartner == null)
                return NotFound();
            else
                return new PlayersController(_service).Get(bestPartner.Name);
        }

        private Player DetermineBestPartner(string name)
        {
            var rankingPerPlayer = new PlayerTrueSkillCalculator().DetermineRanking(_service.Records())
                .ToDictionary(z => z.Player, z => z.Score);

            var player = new Player(name);
            var bestPartner = _service.Records().Games
                .Where(z => z.IsPlayer(player))
                .Where(z => z.IsWinner(player))
                .Select(z => z.Partner(player))
                .GroupBy(z => z)
                .OrderByDescending(z => z.Count())
                .ThenByDescending(z => rankingPerPlayer[z.Key])
                .Select(z => z.Key)
                .FirstOrDefault();
            return bestPartner;
        }

        [HttpGet("api/Stats/PlayerStats/{name}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PlayerStats))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult GetPlayerStats(string name)
        {
            var stats = DeterminePlayerStats().FirstOrDefault(z => new Player(z.Player.Name) == new Player(name));
            if (stats == null)
                return NotFound();
            return Json(stats);
        }

        [HttpGet("api/Stats/PlayingField")]
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
            stats.Blue.WinPercentage = (double)stats.Blue.NumberOfWins / (double)stats.NumberOfGames * 100;
            stats.Red.WinPercentage = (double)stats.Red.NumberOfWins / (double)stats.NumberOfGames * 100;
            return stats;
        }
    }
}
