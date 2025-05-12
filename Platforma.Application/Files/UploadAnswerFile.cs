using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Platforma.Domain;
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
            public required Guid AssignmentId;
            public required Guid UserId;
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
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
                if (assignment == null) return Result<Unit?>.Failure("Assignment not found");
                
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null) return Result<Unit?>.Failure("User not found");

                // Check if assignment require answer
                if (!assignment.AnswerRequired) return Result<Unit?>.Failure("Assignment don't allow answers");

                // Check if answer for specified user and specified assigmnent is already submited
                var answer = _context.Answers
                    .Where(a => a.AssignmentId == request.AssignmentId && a.UserId == request.UserId)
                    .FirstOrDefault();
                if (answer != null) return Result<Unit?>.Failure("Answer already submitted");


                // Check file size
                if (request.File.Length > 1000 * 1024 * 1024) // ~ 1GB
                {
                    return Result<Unit?>.Failure("File size exceeds the limit");
                }

                // Check file type
                var acceptedFileTypes = assignment.AcceptedFileTypes;
                if (acceptedFileTypes != null)
                {
                    List<string> acceptedFileTypesList = acceptedFileTypes.Split(';').ToList();
                    string fileExtension = Path.GetExtension(request.File.FileName);
                    if (!acceptedFileTypesList.Contains(fileExtension))
                    {
                        return Result<Unit?>.Failure("File type not accepted");
                    }
                }

                // Create answer entry
                var newAnswer = new Answer
                {
                    AssignmentId = request.AssignmentId,
                    UserId = request.UserId,
                };

                // Create file path 
                if (assignment.CourseId == Guid.Empty || assignment.Id == Guid.Empty)
                {
                    return Result<Unit?>.Failure("Course or assignment not found.");
                }
                string uploadPath = _configuration["FileStorageConfig:Path"]!;
                string filePath = $"{assignment.CourseId}/answers/{assignment.Id}/{newAnswer.Id}/{Guid.NewGuid().ToString()}_{Path.GetFileName(request.File.FileName)}";
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
                    newAnswer.FilePath = filePath;
                    newAnswer.SubmittedDate = DateTime.Now;
                    _context.Answers.Add(newAnswer);
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
