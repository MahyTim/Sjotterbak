using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sjotterbak.Ranking;
using Sjotterbak.Ranking.EasyStats;
using Sjotterbak.Ranking.Elo;
using Sjotterbak.Ranking.Glicko;
using Sjotterbak.Ranking.TrueSkill;
using Sjotterbak.WebApi.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sjotterbak.WebApi.Controllers
{

    [Produces("application/json")]
    [Route("api/Rankings")]
    [EnableCors("AllowAll")]
    public class RankingController : Controller
    {
        public class RankingResult
        {
            public PlayerRanking[] PlayerRankings { get; set; }
        }

        public class PlayerRanking
        {
            public string Name { get; set; }
            public PlayerRankingRow[] Ranking { get; set; }
        }

        public class PlayerRankingRow
        {
            public string Name { get; set; }
            public double Score { get; set; }
        }

        public class PlayerScoreEvolution
        {
            public PlayersController.Player Player { get; set; }
            public double[] Scores { get; set; }
        }

        private readonly DatabaseService _service;

        public RankingController(DatabaseService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(RankingResult))]
        public IActionResult Get()
        {
            var result = new RankingResult();
            result.PlayerRankings = DeterminePlayerRankings().ToArray();
            return Json(result);
        }

        [HttpGet("PlayerRankingHistory/{name}", Name = "GetPlayerRankingHistoryByName")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PlayerScoreEvolution))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public PlayerScoreEvolution GetPlayerRankingHistory(string name)
        {
            var player = new Player(name);
            var calculator =
                new PlayerRankingHistoryCalculator(new PlayerTrueSkillCalculator(), _service.Records());
            var result = new PlayerScoreEvolution()
            {
                Player = new PlayersController.Player(player),
                Scores = calculator.GetScoringByPlayer(player)
            };
            return result;
        }

        [HttpGet("PlayerRankingHistory", Name = "GetPlayerRankingHistory")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PlayerScoreEvolution[]))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public PlayerScoreEvolution[] GetPlayerRankingHistory()
        {
            var result = new List<PlayerScoreEvolution>();
            foreach (var player in _service.Records().GetPlayers())
            {
                var calculator =
                    new PlayerRankingHistoryCalculator(new PlayerTrueSkillCalculator(), _service.Records());
                result.Add(new PlayerScoreEvolution()
                {
                    Player = new PlayersController.Player(player),
                    Scores = calculator.GetScoringByPlayer(player)
                });
            }
            return result.ToArray();
        }

        private IEnumerable<PlayerRanking> DeterminePlayerRankings()
        {
            var calculators = new List<IPlayerRankingCalculator>()
            {
                new PlayerTrueSkillCalculator(),
                new TimMahyFoosballCalculator(),
                new WinsPercentageCalculator(),
                new PlayerDuelEloFideCalculator(),
                new PlayerDuelEloGaussianCalculator(),
                new LongestWinningStreakCalculator(),
                new GlickoCalculator()
            };

            foreach (var calculator in calculators)
            {
                var ranking = new PlayerRanking()
                {
                    Name = calculator.Name,
                    Ranking = GetRankingResults(calculator)
                };
                yield return ranking;
            }
        }

        private PlayerRankingRow[] GetRankingResults(IPlayerRankingCalculator calculator)
        {
            var results = calculator.DetermineRanking(_service.Records()).ToArray();
            return results.OrderByDescending(z => z.Score)
                .Select(z => new PlayerRankingRow()
                {
                    Name = z.Player.Name,
                    Score = z.Score
                }).ToArray();
        }
    }
}
