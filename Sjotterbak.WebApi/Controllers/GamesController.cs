using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sjotterbak.WebApi.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sjotterbak.WebApi.Controllers
{
    [Produces("application/json")]
    [EnableCors("AllowAll")]
    [Route("api/Games")]
    public class GamesController : Controller
    {
        public class Game
        {
            public Game() { }

            public Game(Sjotterbak.Game inner)
            {
                Blue = new Team()
                {
                    NamePlayerAttacker = inner.Team1Player2.Name,
                    NamePlayerKeeper = inner.Team1Player1.Name
                };
                Red = new Team()
                {
                    NamePlayerAttacker = inner.Team2Player2.Name,
                    NamePlayerKeeper = inner.Team2Player1.Name
                };
                ScoreBlue = inner.ScoreTeam1;
                ScoreRed = inner.ScoreTeam2;
            }

            public Team Blue { get; set; }
            public Team Red { get; set; }
            public int ScoreBlue { get; set; }
            public int ScoreRed { get; set; }

            internal IEnumerable<string> GetPlayerNames()
            {
                yield return Blue.NamePlayerKeeper;
                yield return Blue.NamePlayerAttacker;
                yield return Red.NamePlayerKeeper;
                yield return Red.NamePlayerAttacker;
            }
        }

        public class Team
        {
            public string NamePlayerKeeper { get; set; }
            public string NamePlayerAttacker { get; set; }

        }

        private readonly DatabaseService _service;

        public GamesController(DatabaseService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Game>))]
        public IActionResult Get()
        {
            var data = _service.Records();
            return Json(data.Games.Select(z => new Game(z)));
        }


        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Game))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "If a player or score is missing or draw score is submitted")]
        public IActionResult Post([FromBody]Game newGame)
        {
            var playerNames = newGame.GetPlayerNames().ToArray();
            if (playerNames.Any(string.IsNullOrWhiteSpace))
            {
                return BadRequest("One of the names is invalid");
            }
            var duplicatePlayers = playerNames.GroupBy(z => z.Trim().ToLowerInvariant()).Where(z => z.Count() > 1);
            if (duplicatePlayers.Any())
            {
                return BadRequest("Each player position should be played by a unique player");
            }
            if (newGame.ScoreBlue == newGame.ScoreRed)
            {
                return BadRequest("A draw score result is not allowed");
            }
            if (newGame.ScoreBlue < 0 || newGame.ScoreRed < 0)
            {
                return BadRequest("Negative scores are not allowed");
            }



            var team1player1 = new Player(newGame.Blue.NamePlayerKeeper);
            var team1player2 = new Player(newGame.Blue.NamePlayerAttacker);

            var team2player1 = new Player(newGame.Red.NamePlayerKeeper);
            var team2player2 = new Player(newGame.Red.NamePlayerAttacker);


            var game = _service.Records().AddGame(team1player1, team1player2, team2player1, team2player2, newGame.ScoreBlue, newGame.ScoreRed);

            _service.Records().AuditLogEntries.Add(new AuditLogEntry()
            {
                Message = $"New game added: [{newGame.Blue.NamePlayerKeeper}] [{newGame.Blue.NamePlayerAttacker}] against [{newGame.Red.NamePlayerKeeper}] [{newGame.Red.NamePlayerAttacker}] with a score of {newGame.ScoreBlue} - {newGame.ScoreRed}",
                Timestamp = DateTime.UtcNow
            });

            _service.Persist();
            return Json(new Game(game));
        }
    }
}
