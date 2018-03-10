﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sjotterbak.WebApi.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sjotterbak.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Games")]
    public class GamesController : Controller
    {
        public class Game
        {
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

        // GET: api/Players
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Game>))]
        public IActionResult Get()
        {
            var data = _service.Records();
            return Json(data.Games.Select(z => ToContractType(data, z)));
        }

        private static Game ToContractType(Records data, Sjotterbak.Game z)
        {
            return new Game()
            {
                Blue = new Team()
                {
                    NamePlayerAttacker = data.Expand(data.Expand(z.Team1).PlayerTwo).Name,
                    NamePlayerKeeper = data.Expand(data.Expand(z.Team1).PlayerOne).Name
                },
                Red = new Team()
                {
                    NamePlayerAttacker = data.Expand(data.Expand(z.Team2).PlayerTwo).Name,
                    NamePlayerKeeper = data.Expand(data.Expand(z.Team2).PlayerOne).Name
                },
                ScoreBlue = z.ScoreTeam1,
                ScoreRed = z.ScoreTeam2
            };
        }


        // POST: api/Players
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

            // Create the missing players
            foreach (var playerName in playerNames)
            {
                if (_service.Records().PlayerExists(playerName) == false)
                {
                    _service.Records().AddPlayer(playerName);
                }
            }
            // Form teams
            var teamBlue = _service.Records().GetOrCreateTeam(newGame.Blue.NamePlayerKeeper, newGame.Blue.NamePlayerAttacker);
            var teamRed = _service.Records().GetOrCreateTeam(newGame.Red.NamePlayerKeeper, newGame.Red.NamePlayerAttacker);
            // Write game

            var game = _service.Records().AddGame(teamBlue, teamRed, newGame.ScoreBlue, newGame.ScoreRed);

            _service.Records().AuditLogEntries.Add(new AuditLogEntry()
            {
                Message = $"New game added: [{newGame.Blue.NamePlayerKeeper}] [{newGame.Blue.NamePlayerAttacker}] against [{newGame.Red.NamePlayerKeeper}] [{newGame.Red.NamePlayerAttacker}] with a score of {newGame.ScoreBlue} - {newGame.ScoreRed}",
                Timestamp = DateTime.Now
            });

            _service.Persist();
            return Json(ToContractType(_service.Records(), game));
        }
    }
}