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
    public class EditAssignment
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid CourseId { get; set; }
            public required Guid AssignmentId { get; set; }
            public required AssignmentDTO AssignmentDTO { get; set; }
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
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);

                if (course == null) return Result<Unit?>.Failure("Course not found.");
                if (request.CourseId != request.AssignmentDTO.CourseId) return Result<Unit?>.Failure("Incorrect courseId.");
        
                if (assignment == null) return Result<Unit?>.Failure("Assignment not found.");
                if (request.AssignmentDTO.Id != assignment.Id) return Result<Unit?>.Failure("Incorrect assignmentId."); 

                assignment.Name = request.AssignmentDTO.AssignmentName ?? assignment.Name;
                assignment.Content = request.AssignmentDTO.AssignmentContent ?? assignment.Content;
                assignment.OpenDate = request.AssignmentDTO.OpenDate ?? assignment.OpenDate;
                assignment.Deadline = request.AssignmentDTO.Deadline ?? assignment.Deadline;
                assignment.AcceptedFileTypes = request.AssignmentDTO.AcceptedFileTypes ?? assignment.AcceptedFileTypes;

                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit?>.Failure("Failed to edit assignment.");
                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
