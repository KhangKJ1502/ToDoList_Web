using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Tag
{
    public Tag()
    {
        TaskTags = new HashSet<TaskTag>(); // Add this collection
    }

    public int TagId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual ICollection<TaskTag> TaskTags { get; set; } // Add this collection

    // This will be populated through the TaskTags relationship
    public virtual ICollection<Task> Tasks { get; set; } = new HashSet<Task>();
}
