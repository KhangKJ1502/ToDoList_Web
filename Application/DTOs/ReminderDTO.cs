using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ReminderDTO
    {
        public int? ReminderID { get; set; }
        public int TaskID { get; set; }
        public DateTime ReminderTime { get; set; }
        public bool IsSent { get; set; }
    }
}
