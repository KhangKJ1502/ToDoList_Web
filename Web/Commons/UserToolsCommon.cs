using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace WebUI.Commons
{
    public class UserToolsCommon
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserToolsCommon(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetIdUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var userIdClaim = user?.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            if (!int.TryParse(userIdClaim, out int userId))
                return null;

            return userId;
        }
    }
}
