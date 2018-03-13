using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                this.Name = player.Name;
            }

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
            return Json(_service.Records().GetPlayers().Select(z => new Player(z)));
        }

        // GET: api/Players/5
        [HttpGet("{name}", Name = "Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Player))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult Get(string name)
        {
            var predicate = new Sjotterbak.Player()
            {
                Name = name
            };
            var exists = _service.Records().GetPlayers().Any(z => z == predicate);
            if (exists == false)
            {
                return NotFound();
            }
            return Json(new Player(_service.Records().GetPlayers().First(z => z == predicate)));
        }
    }
}
