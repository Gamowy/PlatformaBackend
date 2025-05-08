using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Application.Assignments.DTO;
using Platforma.Infrastructure;

namespace Platforma.Application.Assignments
{
    public class GetAssignments
    {
        public class Query : IRequest<Result<List<AssignmentDTOResponse>>>
        {
            public required Guid CourseId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<AssignmentDTOResponse>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<AssignmentDTOResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.CourseId);
                if (course == null) return Result<List<AssignmentDTOResponse>>.Failure("Course not found.");
                
                var assignments = await _context.Assignments
                    .Where(x => x.CourseId == request.CourseId)
                    .ToListAsync(cancellationToken);
                if (assignments == null || assignments.Count == 0) return Result<List<AssignmentDTOResponse>>.Failure("No assignments found for this course.");

                var assignmentDTOs = assignments.Select(a => new AssignmentDTOResponse
                {
                    Id = a.Id,
                    CourseId = a.CourseId,
                    AssignmentName = a.Name,
                    AssignmentContent = a.Content,
                    OpenDate = a.OpenDate,
                    Deadline = a.Deadline,
                    AcceptedFileTypes = a.AcceptedFileTypes
                }).ToList();

                return Result<List<AssignmentDTOResponse>>.Success(assignmentDTOs);
            }
        }
    }
}
