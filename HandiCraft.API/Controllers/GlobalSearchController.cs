using HandiCraft.Application.DTOs.Spec;
using HandiCraft.Application.Specificatoins;
using HandiCraft.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HandiCraft.API.Controllers
{
    
    public class GlobalSearchController : APIControllerBase
    {
        private readonly GlobalSearchServices _globalSearchService;

        public GlobalSearchController(GlobalSearchServices globalSearchService)
        {
            _globalSearchService = globalSearchService;
        }

        
        [HttpGet]
        public async Task<ActionResult<GlobalSearchResultDto>> Search([FromQuery] GlobalSearchParams Params)
        {
            if (string.IsNullOrEmpty(Params.Query))
            {
                return BadRequest("Search query is required.");
            }

            var result = await _globalSearchService.GlobalSearchAsync(Params);
            return Ok(result);
        }
    }
}
