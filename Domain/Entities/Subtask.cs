﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Subtask
    {
        public int? SubtaskID { get; set; }
        public int TaskID { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
