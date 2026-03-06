using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrudDemoPratice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalaryPredictionController : ControllerBase
    {
        private readonly ISalaryPredictionService _service;

        public SalaryPredictionController(
            ISalaryPredictionService service)
        {
            _service = service;
        }
        [HttpPost("train")]
        public async Task<IActionResult> Train()
        {
            await _service.TrainModelAsync();
            return Ok("Model trained successfully.");
        }

        [HttpPost("predict")]
        public async Task<IActionResult> Predict(
            SalaryPredictionRequestDto request)
        {
            var result = await _service.PredictAsync(request);
            return Ok(result);
        }
    }
}
