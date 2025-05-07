using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Comment
    {
        public int? CommentID { get; set; }
        public int TaskID { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; }
    }
}
