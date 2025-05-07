using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Task
    {

        public int Id { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int? CategoryId { get; set; }

        public string Status { get; set; } = null!;

        public string? Priority { get; set; }

        public DateTime? DueDate { get; set; }

        //public List<AttachmentDTO>? Attachments { get; set; }

        public Category? Category { get; set; }

        public List<Comment>? Comments { get; set; }

        public List<Reminder>? Reminders { get; set; }

        public List<Subtask>? Subtasks { get; set; }

        public List<Tag>? Tags { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
