using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Platforma.Application.Courses.DTOs;
using Platforma.Domain;
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
                    return Result<FileContentResult>.Failure("Course not found");
                }

                // Create file path to course files
                string uploadPath = _configuration["FileStorageConfig:Path"]!;
                string coursePath = course.Id.ToString();
                string fullPath = Path.Combine(uploadPath, coursePath);

                // Try to send back course files
                var tempFolderPath = "";
                var zipPath = "";
                try
                {
                    var courseStudents = course.Users;
                    var courseOwner = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == course.OwnerId, cancellationToken);
                    var courseOwnerName = courseOwner?.Name ?? "UnknownOwner";
                    var courseName = course.Name;
                    var courseYear = course.AcademicYear.Replace("/", "");

                    var courseAssignments = await _context.Assignments
                        .Where(x => x.CourseId == request.CourseId)
                        .ToListAsync(cancellationToken);

                    if (!Directory.Exists(fullPath))
                    {
                        return Result<FileContentResult>.Failure("Course files not found");
                    }


                    // Create temp folder to store course files
                    tempFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    if (Directory.Exists(tempFolderPath))
                    {
                        Directory.Delete(tempFolderPath, true);
                    }
                    Directory.CreateDirectory(tempFolderPath);

                    // Copy course files to temp folder
                    CopyDirectory(fullPath, tempFolderPath, true);
                    if (Directory.Exists(Path.Combine(tempFolderPath, "answers")))
                    {
                        var answerDirectories = Directory.GetDirectories(Path.Combine(tempFolderPath, "answers"));
                        foreach (var answerDirectory in answerDirectories)
                        {
                            var directoryName = Path.GetFileName(answerDirectory);
                            var assignmentName = courseAssignments
                                .FirstOrDefault(a => a.Id.ToString() == directoryName)?.Name;

                            if (assignmentName != null)
                            {
                                Directory.Move(answerDirectory, Path.Combine(tempFolderPath, "answers", assignmentName));
                                var usersDirectories = Directory.GetDirectories(Path.Combine(tempFolderPath, "answers", assignmentName));
                                foreach (var userDirectory in usersDirectories)
                                {
                                    var userId = Path.GetFileName(userDirectory);
                                    var user = courseStudents?.FirstOrDefault(u => u.Id.ToString() == userId);
                                    if (user != null)
                                    {
                                        Directory.Move(userDirectory, Path.Combine(tempFolderPath, "answers", assignmentName, user.Name));
                                    }
                                    else
                                    {
                                        Directory.Delete(userDirectory, true);
                                    }
                                }
                            }
                            else
                            {
                                Directory.Delete(answerDirectory, true);
                            }
                        }
                    }
                    if (Directory.Exists(Path.Combine(tempFolderPath, "assignments")))
                    {
                        var assignmentDirectories = Directory.GetDirectories(Path.Combine(tempFolderPath, "assignments"));
                        foreach (var assignmentDirectory in assignmentDirectories)
                        {
                            var directoryName = Path.GetFileName(assignmentDirectory);
                            var assignmentName = courseAssignments
                                .FirstOrDefault(a => a.Id.ToString() == directoryName)?.Name;
                            if (assignmentName != null)
                            {
                                Directory.Move(assignmentDirectory, Path.Combine(tempFolderPath, "assignments", assignmentName));
                            }
                            else
                            {
                                Directory.Delete(assignmentDirectory, true);
                            }
                        }
                    }

                    zipPath = Path.Combine(Path.GetTempPath(), $"{courseOwnerName}_{courseName}_{courseYear}.zip");
                    if(File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                    }
                    ZipFile.CreateFromDirectory(tempFolderPath, zipPath, CompressionLevel.Fastest, false);
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
                catch (Exception e)
                {
                    return Result<FileContentResult>.Failure($"Error retrieving course files: {e}");
                }
                finally
                {
                    if (File.Exists(zipPath))
                    {
                        File.Delete(zipPath);
                    }
                    if (Directory.Exists(tempFolderPath))
                    {
                        Directory.Delete(tempFolderPath, true);
                    }
                }
            }
            private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
            {
                // Get information about the source directory
                var dir = new DirectoryInfo(sourceDir);

                // Check if the source directory exists
                if (!dir.Exists)
                    throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

                // Cache directories before we start copying
                DirectoryInfo[] dirs = dir.GetDirectories();

                // Create the destination directory
                Directory.CreateDirectory(destinationDir);

                // Get the files in the source directory and copy to the destination directory
                foreach (FileInfo file in dir.GetFiles())
                {
                    string targetFilePath = Path.Combine(destinationDir, file.Name.Substring(37));
                    file.CopyTo(targetFilePath);
                }

                // If recursive and copying subdirectories, recursively call this method
                if (recursive)
                {
                    foreach (DirectoryInfo subDir in dirs)
                    {
                        string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                        CopyDirectory(subDir.FullName, newDestinationDir, true);
                    }
                }
            }
        }
    }
}
