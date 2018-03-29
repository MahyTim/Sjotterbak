using System;
using System.Collections.Generic;
using System.Linq;
using numl.Model;
using numl.Supervised;
using numl.Supervised.DecisionTree;

namespace Sjotterbak.Prediction.NuML
{
    public class NuMlScorePredictor : IScorePredictor
    {
        private Descriptor _description;
        private IModel _model;

        public class Record
        {

            public Record(Game z)
            {
                Player1 = (z.Team1Player1.Name.GetHashCode() < z.Team1Player2.Name.GetHashCode() ? z.Team1Player1 : z.Team1Player2).Name;
                Player2 = (z.Team1Player1.Name.GetHashCode() < z.Team1Player2.Name.GetHashCode() ? z.Team1Player2 : z.Team1Player1).Name;
                Player3 = (z.Team2Player1.Name.GetHashCode() < z.Team2Player2.Name.GetHashCode() ? z.Team2Player1 : z.Team2Player2).Name;
                Player4 = (z.Team2Player1.Name.GetHashCode() < z.Team2Player2.Name.GetHashCode() ? z.Team2Player2 : z.Team2Player1).Name;
                Score = z.ScoreTeam1 - z.ScoreTeam2;
            }

            [Feature]
            public string Player1 { get; set; }
            [Feature]
            public string Player2 { get; set; }
            [Feature]
            public string Player3 { get; set; }
            [Feature]
            public string Player4 { get; set; }
            [Label]
            public int Score { get; set; }
        }

        public PredictionResult Predict(Game game)
        {
            var predictedScore = _model.Predict(new Record(game));
            return new PredictionResult()
            {
                ScoreDifference = predictedScore.Score,
                HumanReadableResult = DetermineHumanReadableResult(game, predictedScore)
            };
        }

        private string DetermineHumanReadableResult(Game game, Record predictedScore)
        {
            var winningTeam = $"Blue ({game.Team1Player1}, {game.Team1Player2})";
            if (predictedScore.Score < 0)
            {
                winningTeam = $"Red ({game.Team2Player1}, {game.Team2Player2})";
            }
            return $"{winningTeam} team wins with a score difference of {Math.Abs(predictedScore.Score)}";
        }

        public void Train(IEnumerable<Game> games)
        {
            _description = Descriptor.Create<Record>();
            var generator = new DecisionTreeGenerator();
            var data = games.Select(z => new Record(z));
            _model = generator.Generate(_description, data);
        }

        public string Name => "NuMLScorePredictor";
    }
}
