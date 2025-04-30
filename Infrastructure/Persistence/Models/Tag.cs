using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Tag
{
    public int TagId { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
