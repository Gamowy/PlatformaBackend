using MediatR;
using Microsoft.Extensions.Configuration;
using Platforma.Infrastructure;

namespace Platforma.Application.Files
{
    public class RemoveAssignmentFile
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid AssignmentId { get; set; }
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
                var assignment = _context.Assignments.Find(request.AssignmentId);
                if (assignment == null)
                {
                    return Result<Unit?>.Failure("Assignment not found");
                }
                string uploadPath = _configuration["FileStorageConfig:Path"]!;
                string filePath = assignment.FilePath;
                string fullPath = Path.Combine(uploadPath, filePath);

                // Clear file path reference in database
                assignment.FilePath = "";
                assignment.FileName = "";
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                // Delete file
                Console.WriteLine(fullPath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    FilesUtils.DeleteEmptyDirectoriesUpwards(fullPath);
                }
                if (!result) return Result<Unit?>.Failure("Failed to delete assignment file");
                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
