using Microsoft.AspNetCore.Http;

namespace Platforma.Application.Files
{
    public class AssignmentUploadDTO
    {
        public Guid AssignmentId { get; set; }
        public IFormFile File { get; set; }
    }
}
