using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual User User { get; set; } = null!;
}
