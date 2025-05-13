using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Courses.DTOs
{
    public class CourseCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string AcademicYear { get; set; }
    }
}
