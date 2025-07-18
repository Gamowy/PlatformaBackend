﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Platforma.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Platforma.Application.Files
{
    public class UploadAssignmentFile
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid AssignmentId;
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
                if (assignment == null)
                {
                    return Result<Unit?>.Failure("Assignment not found");
                }
                if (!string.IsNullOrEmpty(assignment.FilePath))
                {
                    return Result<Unit?>.Failure("File already uploaded");
                }

                // Check file size
                int maxFileSize = Int32.Parse(_configuration["FileStorageConfig:MaxFileSize"]!);
                if (request.File.Length > maxFileSize)
                {
                    return Result<Unit?>.Failure("File size exceeds the limit");
                }

                // Create file path 
                string uploadPath = _configuration["FileStorageConfig:Path"]!;

                string filePath = $"{assignment.CourseId}/assignments/{assignment.Id}/{Guid.NewGuid().ToString()}_{Path.GetFileName(request.File.FileName)}";
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
                    assignment.FilePath = filePath;
                    assignment.FileName = request.File.FileName;
                    var result = await _context.SaveChangesAsync() > 0;
                    if (!result)
                    {
                        throw new Exception("Failed to save file path refrence in database");
                    }
                }
                catch (Exception ex)
                {
                    FilesUtils.RollbackFileUpload(fullPath);
                    return Result<Unit?>.Failure($"Error saving file: {ex}");
                }
                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
