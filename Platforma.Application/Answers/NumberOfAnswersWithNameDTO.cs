using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Answers
{
    public class NumberOfAnswersWithNameDTO
    {
        public Guid AssignmentId { get; set; }
        public Guid CourseId { get; set; }
        public int UnfinishedAnswers { get; set; }
        public int FinishedAnswers { get; set; }
        public string CourseName { get; set; }
        public string AssigmentName { get; set; }
        public DateTime? Deadline { get; set; }

    }
}



