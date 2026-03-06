using CrudDemoPratice.Models.MLModels;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.ML.Trainer
{
    public class SalaryModelTrainer
    {

        public void Train(IEnumerable<SalaryTrainingData> data, string modelPath)
        {
            var mlContext = new MLContext();

            var trainingData = mlContext.Data.LoadFromEnumerable(data);

            var pipeline =
                mlContext.Transforms.Categorical.OneHotEncoding(
                    outputColumnName: "DepartmentEncoded",
                    inputColumnName: nameof(SalaryTrainingData.Department))

                .Append(mlContext.Transforms.Concatenate(
                    "Features",
                    nameof(SalaryTrainingData.Age),
                    nameof(SalaryTrainingData.ExperienceYears),
                    "DepartmentEncoded"))

                .Append(mlContext.Regression.Trainers.FastTree());

            var model = pipeline.Fit(trainingData);

            mlContext.Model.Save(model, trainingData.Schema, modelPath);
        }
    }
}
