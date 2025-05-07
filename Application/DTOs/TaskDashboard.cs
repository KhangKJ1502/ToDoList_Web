using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TaskDashboard
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Priority {  get; set; }
        public DateOnly dueDate { get; set; }
        public int UserId {  get; set; }

    }
}
