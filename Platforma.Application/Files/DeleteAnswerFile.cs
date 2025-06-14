using MediatR;
using Microsoft.Extensions.Configuration;
using Platforma.Infrastructure;

namespace Platforma.Application.Files
{
    public class DeleteAnswerFile
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid AnswerId { get; set; }
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
                var answer = _context.Answers.Find(request.AnswerId);
                if (answer == null)
                {
                    return Result<Unit?>.Failure("Answer not found");
                }
                string uploadPath = _configuration["FileStorageConfig:Path"]!;
                string filePath = answer.FilePath;
                string fullPath = Path.Combine(uploadPath, filePath);

               _context.Remove(answer);
               var result = await _context.SaveChangesAsync() > 0;

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
