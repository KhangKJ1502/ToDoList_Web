using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers.Api
{
    [ApiController]
    [Route("api/")]
    public class TagController : ControllerBase
    {
        public async Task<IActionResult> GetAllTags()
        {
            return Ok();
        }
    }
}
