using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Files
{
    public class AssignmentUploadDTO
    {
        public Guid AssignmentId { get; set; }
        public Guid CourseId { get; set; }
        public IFormFile File { get; set; }
    }
}
