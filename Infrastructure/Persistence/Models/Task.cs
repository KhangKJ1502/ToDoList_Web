using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Models;

public partial class Task
{
    public Task()
    {
        Attachments = new HashSet<Attachment>();
        Comments = new HashSet<Comment>();
        Reminders = new HashSet<Reminder>();
        Subtasks = new HashSet<Subtask>();
        TaskTags = new HashSet<TaskTag>(); // Add this collection
    }

    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int? CategoryId { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual Category Category { get; set; }
    public virtual ICollection<Attachment> Attachments { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }
    public virtual ICollection<Reminder> Reminders { get; set; }
    public virtual ICollection<Subtask> Subtasks { get; set; }
    public virtual ICollection<TaskTag> TaskTags { get; set; } // Add this collection

    // This will be populated through the TaskTags relationship
    public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

}
