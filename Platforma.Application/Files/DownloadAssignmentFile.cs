using HeyRed.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Platforma.Infrastructure;

namespace Platforma.Application.Files
{
    public class DownloadAssignmentFile
    {
        public class Query : IRequest<Result<FileContentResult>>
        {
            public Guid AssignmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<FileContentResult>>
        {
            private readonly DataContext _context;
            private readonly IConfiguration _configuration;

            public Handler(DataContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }
            public async Task<Result<FileContentResult>> Handle(Query request, CancellationToken cancellationToken)
            {
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
                if (assignment == null)
                {
                    return Result<FileContentResult>.Failure("Assignment not found");
                }
                if (assignment.FilePath == null)
                {
                    return Result<FileContentResult>.Failure("No file uploaded for assignment");
                }

                // Create file path to assignment file
                string uploadPath = _configuration["FileStorageConfig:Path"]!;
                string fullPath = Path.Combine(uploadPath, assignment.FilePath);

                // Try to send back the file
                try
                {
                    if (!File.Exists(fullPath))
                    {
                        return Result<FileContentResult>.Failure("File not found");
                    }
                    var mimeType = MimeTypesMap.GetMimeType(fullPath);
                    byte[]? fileBytes;

                    // Retrieve file
                    using (var stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            fileBytes = memoryStream.ToArray();
                        }
                        if (fileBytes != null)
                        {
                            var file = new FileContentResult(fileBytes, mimeType)
                            {
                                FileDownloadName = Path.GetFileName(fullPath).Substring(37)
                            };
                            return Result<FileContentResult>.Success(file);
                        }
                        return Result<FileContentResult>.Failure("Failed to retrive file");
                    }
                }
                catch
                {
                    return Result<FileContentResult>.Failure("Error retrieving file");
                }
            }
        }
    }
}
