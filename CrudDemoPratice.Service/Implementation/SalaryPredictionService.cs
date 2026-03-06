using CrudDemoPratice.ML.Predictor;
using CrudDemoPratice.ML.Trainer;
using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Models.MLModels;
using CrudDemoPratice.Repository.Interface;
using CrudDemoPratice.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Service.Implementation
{
    public class SalaryPredictionService:ISalaryPredictionService
    {
        private readonly IEmployeeRepo _repository;
        private readonly string _modelPath;
        public SalaryPredictionService(IEmployeeRepo repository)
        {
            _repository = repository;
            _modelPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "MLModels",
                "salary_model.zip");
        }



        public async Task TrainModelAsync()
        {
            var employees = await _repository.GetAllAsyncRepo();

            var trainingData = employees.Select(e =>
            {
                var experience = CalculateExperience(e.JoiningDate);

                return new SalaryTrainingData
                {
                    Age = e.Age,
                    ExperienceYears = experience,
                    Department = e.Department,
                    Salary = e.Salary
                };
            });

            var trainer = new SalaryModelTrainer();
            trainer.Train(trainingData, _modelPath);
        }



        public async Task<SalaryPredictionResponseDto> PredictAsync(
            SalaryPredictionRequestDto request)
        {
            var predictor = new SalaryModelPredictor(_modelPath);

            var experience = CalculateExperience(request.JoiningDate);

            var input = new SalaryTrainingData
            {
                Age = request.Age,
                ExperienceYears = experience,
                Department = request.Department
            };

            var predictedSalary = predictor.Predict(input);

            return new SalaryPredictionResponseDto
            {
                PredictedSalary = predictedSalary
            };
        }


        private float CalculateExperience(DateOnly joiningDate)
        {
            var joiningDateTime = joiningDate.ToDateTime(TimeOnly.MinValue);
            var years = (DateTime.Now - joiningDateTime).TotalDays / 365;
            return (float)years;
        }


    }
}
