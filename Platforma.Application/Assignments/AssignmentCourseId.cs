using MediatR;
using Platforma.Infrastructure;

namespace Platforma.Application.Assignments
{
    public class AssignmentCourseId
    {
        public class Query : IRequest<Result<Guid>>
        {
            public required Guid AssignmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Guid>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Guid>> Handle(Query request, CancellationToken cancellationToken)
            {
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
                if (assignment == null) return Result<Guid>.Failure("Assignment not found");
                return Result<Guid>.Success(assignment.CourseId);
            }
        }
    }
}
