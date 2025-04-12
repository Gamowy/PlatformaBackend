using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Domain
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime Deadline { get; set; }
        public string FilePath { get; set; }
        public bool AnswerRequired { get; set; }
        public string AcceptedFileTypes { get; set; }
    }
}
