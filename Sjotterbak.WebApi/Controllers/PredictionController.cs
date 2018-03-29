using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sjotterbak.Prediction.NuML;
using Sjotterbak.WebApi.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sjotterbak.WebApi.Controllers
{
    [Produces("application/json")]
    [EnableCors("AllowAll")]
    [Route("api/Prediction")]
    public class PredictionController : Controller
    {
        private readonly DatabaseService _service;

        public PredictionController(DatabaseService service)
        {
            _service = service;
        }

        public class Prediction
        {
            public string HumanReadableResult { get; set; }
            public int ScoreDifference { get; set; }
            public string Algorithm { get; set; }
        }

        [HttpGet("/Score/{BlueKeeper}/{BlueAttacker}/{RedKeeper}/{RedAttacker}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Prediction[]))]
        public Prediction[] Get(string blueKeeper,string blueAttacker,string redKeeper,string redAttacker)
        {

            var data = _service.Records();
            var result = new List<Prediction>();
            var algorithms = new[] {new NuMlScorePredictor()};
            foreach (var algorithm in algorithms)
            {
                algorithm.Train(data.Games);

                var prediction = algorithm.Predict(new Game()
                {
                    Team1Player1 = new Player(blueKeeper),
                    Team2Player1 = new Player(redKeeper),
                    Team1Player2 = new Player(blueAttacker),
                    Team2Player2 = new Player(redAttacker)
                });

                result.Add(new Prediction()
                {
                    Algorithm = algorithm.Name,
                    HumanReadableResult = prediction.HumanReadableResult,
                    ScoreDifference = prediction.ScoreDifference,
                });
            }

            return result.ToArray();
        }
    }
}