using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }  // Mật khẩu sẽ được mã hóa trước khi lưu
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int UserId { get; set; }
    }
}
