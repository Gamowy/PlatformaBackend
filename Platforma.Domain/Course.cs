using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Domain
{
    public class Course
    {
        public Guid Id { get; set; }
        public List<User>? Users { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }
        public string AcademicYear { get; set; }

        public List<Assignment>? Assignments { get; set; }
    }
}
