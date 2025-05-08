using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebUI.Commons;

namespace WebUI.Controllers.Api
{
    [ApiController]
    [Route("api/")]
    public class TagController : ControllerBase
    {
        private readonly ITagService tagService;
        private readonly UserToolsCommon userToolsCommon;

        public TagController(ITagService tagService, UserToolsCommon toolsCommon)
        {
            this.tagService = tagService;
            this.userToolsCommon = toolsCommon;
        }

        [HttpGet]
        [Route("tags")]
        public async Task<IActionResult> GetAllTags() 
        {
            int? userId = userToolsCommon.GetIdUser();

            if (userId == null) 
                return Unauthorized();

            var Tags = await tagService.GetAllsTags(userId.Value); 

            if (Tags == null) {
                return BadRequest(new {success = false, message ="Khong the tai trang" });
            }
            return Ok(new {success = true, 
                message="Tai thanh cong",
                data=Tags
            });
        }
    }
}
