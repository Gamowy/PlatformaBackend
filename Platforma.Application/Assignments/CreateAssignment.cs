using MediatR;
using Platforma.Application.Assignments.DTO;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Assignments
{
    public class CreateAssignment
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required AssignmentDTORequest AssignmentDTO { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit?>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Unit?>> Handle(Command request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.AssignmentDTO.CourseId);
                if (course == null) return Result<Unit?>.Failure("Course not found.");
                if (request.AssignmentDTO.CourseId != request.AssignmentDTO.CourseId) return Result<Unit?>.Failure("Incorrect courseId.");

                var assignment = new Assignment
                {
                    CourseId = request.AssignmentDTO.CourseId,
                    Name = request.AssignmentDTO.AssignmentName ?? "New assignment",
                    Content = request.AssignmentDTO.AssignmentContent ?? "Assigment description",
                    OpenDate = DateTime.Now,
                    Deadline = request.AssignmentDTO.Deadline ?? DateTime.Now.AddDays(7),
                    FilePath = "",
                    AcceptedFileTypes = request.AssignmentDTO.AcceptedFileTypes ?? ".txt;.docx;.pdf;.zip",
                };
                _context.Assignments.Add(assignment);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit?>.Failure("Failed to create assignment.");
                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
