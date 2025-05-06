using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CategoriesDTO
    {
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Color { get; set; }
        public int UserId { get; set; } // ⚠️ Bắt buộc phải có nếu có FOREIGN KEY
    }
}