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


        private readonly DatabaseService _service;

        public RankingController(DatabaseService service)
        {
            _service = service;
        }

        // GET: api/Players
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(RankingResult))]
        public IActionResult Get()
        {
            var result = new RankingResult();
            result.PlayerRankings = DeterminePlayerRankings().ToArray();
            return Json(result);
        }

        private IEnumerable<PlayerRanking> DeterminePlayerRankings()
        {
            var calculators = new List<IPlayerRankingCalculator>()
            {
                new TimMahyFoosballCalculator(),
                new PlayerTrueSkillCalculator(),
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
