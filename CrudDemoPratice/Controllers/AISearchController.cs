using CrudDemoPratice.Models.DTOs.AISearch;
using CrudDemoPratice.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrudDemoPratice.Controllers
{
    [ApiController]
    [Route("api/ai-search")]
    public class AISearchController : ControllerBase
    {
        private readonly IAISearchService _service;

        public AISearchController(IAISearchService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Search(AISearchRequestDto request)
        {
            var result = await _service.SearchAsync(request.NaturalLanguageQuery);
            return Ok(result);
        }
    }

}
