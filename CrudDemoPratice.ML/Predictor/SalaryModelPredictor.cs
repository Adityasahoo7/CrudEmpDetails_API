using CrudDemoPratice.Models.MLModels;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.ML.Predictor
{
    public class SalaryModelPredictor
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private PredictionEngine<SalaryTrainingData, SalaryPredictionResult>? _engine;

        public SalaryModelPredictor(string modelPath)
        {
            _mlContext = new MLContext();

            if (File.Exists(modelPath))
            {
                _model = _mlContext.Model.Load(modelPath, out _);
                _engine = _mlContext.Model.CreatePredictionEngine
                    <SalaryTrainingData, SalaryPredictionResult>(_model);
            }
        }

        public float Predict(SalaryTrainingData input)
        {
            if (_engine == null)
                throw new Exception("Model not trained yet.");

            var result = _engine.Predict(input);
            return result.Score;
        }
    }
}
