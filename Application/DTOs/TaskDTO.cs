using Infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    namespace Application.DTOs
    {
        public class TaskDTO
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

            //public CategoryDTO? Category { get; set; }

            //public List<CommentDTO>? Comments { get; set; }

            //public List<ReminderDTO>? Reminders { get; set; }

            //public List<SubtaskDTO>? Subtasks { get; set; }

            //public List<TagDTO>? Tags { get; set; }
        }
    }
}

