using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Answers
{
    public class ListOfStudentWithAnswearDTO
    {
        public Guid UserId { get; set; }
        public Guid? AnswerId { get; set; }
        public bool IsSubmitted { get; set; }
        public string UserName { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public float? Mark { get; set; }
        public string? Comment { get; set; }

    }
}
