using Platforma.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Platforma.Application.Courses.DTOs
{
    public class CourseEditDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AcademicYear { get; set; }
        public Guid? OwnerId { get; set; }
    }
}
