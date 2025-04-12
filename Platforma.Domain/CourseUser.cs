using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Domain
{
    public enum UserStatus { Awaiting, Accepted, Rejected }
    
    public class CourseUser
    {
        public Guid CourseID { get; set; }
        public Guid UserID { get; set; }
        public UserStatus Status { get; set; }
    }
}
