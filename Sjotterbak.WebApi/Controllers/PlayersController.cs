using System;
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
    [Route("api/Players")]
    public class PlayersController : Controller
    {
        public class NewPlayer
        {
            public string Name { get; set; }
        }

        public class Player
        {
            public Player() { }

            public Player(Sjotterbak.Player player)
            {
                this.Id = player.Id.Value;
                this.Name = player.Name;
            }

            public int Id { get; set; }
            public string Name { get; set; }
        }

        private readonly DatabaseService _service;

        public PlayersController(DatabaseService service)
        {
            _service = service;
        }

        // GET: api/Players
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Player>))]
        public IActionResult Get()
        {
            return Json(_service.Records().Players.Select(z => new Player(z)));
        }

        // GET: api/Players/5
        [HttpGet("{id}", Name = "Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Player))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult Get(int id)
        {
            var exists = _service.Records().Players.Any(z => z.Id.Value == id);
            if (exists == false)
            {
                return NotFound();
            }
            return Json(new Player(_service.Records().Players.First(z => z.Id.Value == id)));
        }

        // POST: api/Players
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Player))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "If the player already exists or empty name was given")]
        public IActionResult Post([FromBody]NewPlayer newPlayer)
        {
            if (string.IsNullOrWhiteSpace(newPlayer.Name))
            {
                return BadRequest("Name is invalid");
            }
            var exists = _service.Records().PlayerExists(newPlayer.Name);
            if (exists)
            {
                return BadRequest("Player already exists");
            }

            var result = _service.Records().AddPlayer(newPlayer.Name);
            _service.Persist();
            return Json(result);
        }
    }
}
