using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Assignments.DTO
{
    public class AssignmentDTORequest
    {
        public Guid CourseId { get; set; }
        public string? AssignmentName { get; set; }
        public string? AssignmentContent { get; set; }
        public DateTime? Deadline { get; set; }
        public string? AcceptedFileTypes { get; set; }
    }
}
