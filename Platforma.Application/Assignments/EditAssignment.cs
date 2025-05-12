using MediatR;
using Platforma.Infrastructure;

namespace Platforma.Application.Assignments
{
    public class EditAssignment
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid AssignmentId { get; set; }
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
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
                if (assignment == null) return Result<Unit?>.Failure("Assignment not found.");
                if (request.AssignmentDTO.OpenDate.HasValue && request.AssignmentDTO.Deadline.HasValue)
                {
                    if (request.AssignmentDTO.OpenDate > request.AssignmentDTO.Deadline)
                        return Result<Unit?>.Failure("Open date cannot be after deadline.");
                }

                assignment.Name = request.AssignmentDTO.AssignmentName ?? assignment.Name;
                assignment.Content = request.AssignmentDTO.AssignmentContent ?? assignment.Content;
                assignment.OpenDate = request.AssignmentDTO.OpenDate ?? assignment.OpenDate;
                assignment.Deadline = request.AssignmentDTO.Deadline ?? assignment.Deadline;
                assignment.AcceptedFileTypes = request.AssignmentDTO.AcceptedFileTypes ?? assignment.AcceptedFileTypes;
                assignment.AnswerRequired = request.AssignmentDTO.AnswerRequired ?? assignment.AnswerRequired;

                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit?>.Failure("Failed to edit assignment.");
                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
