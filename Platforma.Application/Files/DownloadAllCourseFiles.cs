using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Platforma.Infrastructure;
using System.IO.Compression;

namespace Platforma.Application.Files
{
    public class DownloadAllCourseFiles
    {
        public class Query : IRequest<Result<FileContentResult>>
        {
            public Guid CourseId { get; set; }
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
                var course = await _context.Courses.FindAsync(request.CourseId);
                if (course == null)
                {
                    return Result<FileContentResult>.Failure("Answer not found");
                }

                // Create file path to course files
                string uploadPath = _configuration["FileStorageConfig:Path"]!;
                string coursePath = course.Id.ToString();
                string fullPath = Path.Combine(uploadPath, coursePath);

                // Try to send back course files
                try
                {
                    if (!Directory.Exists(fullPath))
                    {
                        return Result<FileContentResult>.Failure("Course files not found");
                    }
                    // Create the ZIP file
                    var zipPath = Path.Combine(uploadPath, $"{course.Name}.zip");
                    DeleteZip(zipPath);
                    ZipFile.CreateFromDirectory(fullPath, zipPath, CompressionLevel.Fastest, true);

                    var mimeType = "application/zip";
                    byte[]? fileBytes;

                    // Retrieve file
                    using (var stream = new FileStream(zipPath, FileMode.Open))
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
                                FileDownloadName = Path.GetFileName(zipPath)
                            };
                            return Result<FileContentResult>.Success(file);
                        }
                        return Result<FileContentResult>.Failure("Failed to retrive course files");
                    }
                }
                catch
                {
                    return Result<FileContentResult>.Failure("Error retrieving course files");
                }
                finally
                {
                    DeleteZip(Path.Combine(uploadPath, $"{course.Name}.zip"));
                }
            }

            private void DeleteZip(string zipPath)
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
            }
        }
    }
}
