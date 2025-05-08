using MediatR;
using Platforma.Domain;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Assignments
{
    public class DeleteAssignment
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid CourseId { get; set; }
            public required Guid AssignmentId { get; set; }
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
                var course = await _context.Courses.FindAsync(request.CourseId);
                if (course == null) return Result<Unit?>.Failure("Course not found.");
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
                if (assignment == null) return Result<Unit?>.Failure("Assignment not found.");
                _context.Assignments.Remove(assignment);
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit?>.Failure("Failed to delete assignment.");
                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
