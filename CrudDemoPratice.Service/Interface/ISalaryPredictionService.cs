using CrudDemoPratice.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Service.Interface
{
    public interface ISalaryPredictionService
    {
        Task TrainModelAsync();

        Task<SalaryPredictionResponseDto> PredictAsync(
            SalaryPredictionRequestDto request);
    }
}
