using System;
using System.Collections.Generic;
using System.Text;

namespace Sjotterbak.Prediction
{
    public interface IScorePredictor
    {
        PredictionResult Predict(Game game);
        void Train(IEnumerable<Game> games);
        string Name { get;  }
    }
}
