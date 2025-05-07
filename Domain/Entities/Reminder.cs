using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Reminder
    {
        public int? ReminderID { get; set; }
        public int TaskID { get; set; }
        public DateTime ReminderTime { get; set; }
        public bool IsSent { get; set; }
    }
}
