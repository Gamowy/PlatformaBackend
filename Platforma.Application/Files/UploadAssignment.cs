using MediatR;
using Microsoft.AspNetCore.Http;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace Platforma.Application.Files
{
    public class UploadAssignment
    {
        public class Command : IRequest<Result<Unit>>
        {
            public required AssignmentUploadDTO AssignmentUploadDTO { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IConfiguration _configuration;

            public Handler(DataContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var course = _context.Courses.Find(request.AssignmentUploadDTO.CourseId);
                if (course == null)
                {
                    return Result<Unit>.Failure("Course not found");
                }
                var assignment = _context.Assignments.Find(request.AssignmentUploadDTO.AssignmentId);
                if (assignment == null)
                {
                    return Result<Unit>.Failure("Assignment not found");
                }
                if (!string.IsNullOrEmpty(assignment.FilePath))
                {
                    return Result<Unit>.Failure("File already uploaded");
                }


                // Check file size
                if (request.AssignmentUploadDTO.File.Length > 1000 * 1024 * 1024) // ~ 1GB
                {
                    return Result<Unit>.Failure("File size exceeds the limit");
                }

                // Create file path 
                string uploadPath = _configuration["FileStorageConfig:Path"]!;

                string filePath = $"assignments/{request.AssignmentUploadDTO.CourseId}/{Guid.NewGuid().ToString()}_{request.AssignmentUploadDTO.File.FileName}";
                string fullPath = Path.Combine(uploadPath, filePath);

                // Save file
                try
                {
                    // Create directory if it doesn't exist yet
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                    // Save file to storage
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await request.AssignmentUploadDTO.File.CopyToAsync(stream);
                    }
                    // Save file path reference in database
                    assignment.FilePath = filePath;
                    var result = await _context.SaveChangesAsync() > 0;
                    if (!result)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    RollbackFileUpload(fullPath);
                    return Result<Unit>.Failure($"Error saving file: {ex}");
                }
                return Result<Unit>.Success(Unit.Value);
            }

            // Rollback file upload if error occurs
            private void RollbackFileUpload(string fullPath)
            {
                try
                {
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                catch (Exception ex) { 
                    Console.WriteLine($"Error deleting file: {ex.Message}");
                }
            }
        }
    }
}
