using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Application.Assignments;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Assignments
{
    public class GetAssignments
    {
        public class Query : IRequest<Result<List<Assignment>>>
        {
            public required Guid CourseId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Assignment>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<Assignment>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.CourseId);
                if (course == null) return Result<List<Assignment>>.Failure("Course not found.");
                
                var assignments = await _context.Assignments
                    .Where(x => x.CourseId == request.CourseId)
                    .ToListAsync(cancellationToken);
                if (assignments == null || assignments.Count == 0) return Result<List<Assignment>>.Failure("No assignments found for this course.");

                return Result<List<Assignment>>.Success(assignments);
            }
        }
    }
}
