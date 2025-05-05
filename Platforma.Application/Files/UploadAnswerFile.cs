using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Files
{
    public class UploadAnswerFile
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid AnswerId;
            public required IFormFile File;
        }

        public class Handler : IRequestHandler<Command, Result<Unit?>>
        {
            private readonly DataContext _context;
            private readonly IConfiguration _configuration;

            public Handler(DataContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<Result<Unit?>> Handle(Command request, CancellationToken cancellationToken)
            {
                var answer = await _context.Answers.FindAsync(request.AnswerId);
                if (answer == null)
                {
                    return Result<Unit?>.Failure("Answer not found");
                }
                if (!string.IsNullOrEmpty(answer.FilePath))
                {
                    return Result<Unit?>.Failure("File already uploaded");
                }

                // Check file size
                if (request.File.Length > 1000 * 1024 * 1024) // ~ 1GB
                {
                    return Result<Unit?>.Failure("File size exceeds the limit");
                }

                // Check file type
                var acceptedFileTypes = _context.Assignments
                    .Where(a => a.Answers.Any(ans => ans.Id == request.AnswerId))
                    .Select(a => a.AcceptedFileTypes)
                    .FirstOrDefault();
                if (acceptedFileTypes != null)
                {
                    List<string> acceptedFileTypesList = acceptedFileTypes.Split(';').ToList();
                    string fileExtension = Path.GetExtension(request.File.FileName);
                    if (!acceptedFileTypesList.Contains(fileExtension))
                    {
                        return Result<Unit?>.Failure("File type not accepted");
                    }
                }

                // Create file path 
                var courseId = _context.Assignments
                    .Where(a => a.Answers.Any(ans => ans.Id == request.AnswerId))
                    .Select(a => a.CourseId)
                    .FirstOrDefault();
                var assignmentId = _context.Assignments
                    .Where(a => a.Answers.Any(ans => ans.Id == request.AnswerId))
                    .Select(a => a.Id)
                    .FirstOrDefault();
                if (courseId == Guid.Empty || assignmentId == Guid.Empty)
                {
                    return Result<Unit?>.Failure("Course or assignment not found.");
                }
                string uploadPath = _configuration["FileStorageConfig:Path"]!;
                string filePath = $"answers/{courseId}/{assignmentId}/{Guid.NewGuid().ToString()}_{request.File.FileName}";
                string fullPath = Path.Combine(uploadPath, filePath);

                // Save file
                try
                {
                    // Create directory if it doesn't exist yet
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                    // Save file to storage
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await request.File.CopyToAsync(stream);
                    }
                    // Save file path reference in database
                    answer.FilePath = filePath;
                    answer.SubmittedDate = DateTime.UtcNow;
                    var result = await _context.SaveChangesAsync() > 0;
                    if (!result)
                    {
                        throw new Exception("Failed to save file path refrence in database");
                    }
                }
                catch (Exception ex)
                {
                    RollbackFileUpload(fullPath);
                    return Result<Unit?>.Failure($"Error saving file: {ex}");
                }
                return Result<Unit?>.Success(Unit.Value);
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
                catch
                {
                    Console.WriteLine($"Error deleting file durign rollback");
                }
            }
        }

    }
}
