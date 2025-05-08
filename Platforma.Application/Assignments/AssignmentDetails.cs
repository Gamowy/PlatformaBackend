using MediatR;
using Platforma.Domain;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Assignments
{
    public class AssignmentDetails
    {
        public class Query : IRequest<Result<AssignmentDTO>>
        {
            public required Guid CourseId { get; set; }
            public required Guid AssignmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<AssignmentDTO>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<AssignmentDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.CourseId);
                if (course == null) return Result<AssignmentDTO>.Failure("Course not found");
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
                if (assignment == null) return Result<AssignmentDTO>.Failure("Assignment not found");

                var assignmentDTO = new AssignmentDTO
                {
                    Id = assignment.Id,
                    CourseId = assignment.CourseId,
                    AssignmentName = assignment.Name,
                    AssignmentContent = assignment.Content,
                    OpenDate = assignment.OpenDate,
                    Deadline = assignment.Deadline,
                    AcceptedFileTypes = assignment.AcceptedFileTypes
                };

                return Result<AssignmentDTO>.Success(assignmentDTO);
            }
        }
    }
}
