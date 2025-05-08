using MediatR;
using Platforma.Application.Assignments.DTO;
using Platforma.Infrastructure;

namespace Platforma.Application.Assignments
{
    public class AssignmentDetails
    {
        public class Query : IRequest<Result<AssignmentDTOResponse>>
        {
            public required Guid AssignmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<AssignmentDTOResponse>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<AssignmentDTOResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
                if (assignment == null) return Result<AssignmentDTOResponse>.Failure("Assignment not found");

                var assignmentDTO = new AssignmentDTOResponse
                {
                    Id = assignment.Id,
                    CourseId = assignment.CourseId,
                    AssignmentName = assignment.Name,
                    AssignmentContent = assignment.Content,
                    OpenDate = assignment.OpenDate,
                    Deadline = assignment.Deadline,
                    AcceptedFileTypes = assignment.AcceptedFileTypes
                };

                return Result<AssignmentDTOResponse>.Success(assignmentDTO);
            }
        }
    }
}
