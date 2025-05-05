using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Files
{
    public class AnswerUploadDTO
    {
        public Guid AnswerId { get; set; }
        public IFormFile File { get; set; }
    }
}
