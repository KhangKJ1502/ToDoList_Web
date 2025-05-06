using System;

namespace Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}