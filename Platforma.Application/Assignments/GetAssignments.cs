using MediatR;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Assignments
{
    public class GetAssignments
    {
        public class Query : IRequest<Result<List<AssignmentDTO>>>
        {
            public required Guid CourseId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<AssignmentDTO>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<AssignmentDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.CourseId);
                if (course == null) return Result<List<AssignmentDTO>>.Failure("Course not found.");
                var assignments = _context.Assignments
                    .Where(a => a.CourseId == request.CourseId)
                    .ToList();
                if (assignments == null) return Result<List<AssignmentDTO>>.Failure("Assignments not found.");

                var assignmentDTOs = assignments.Select(a => new AssignmentDTO
                {
                    Id = a.Id,
                    CourseId = a.CourseId,
                    AssignmentName = a.Name,
                    AssignmentContent = a.Content,
                    OpenDate = a.OpenDate,
                    Deadline = a.Deadline,
                    AcceptedFileTypes = a.AcceptedFileTypes
                }).ToList();

                return Result<List<AssignmentDTO>>.Success(assignmentDTOs);
            }
        }
    }
}
