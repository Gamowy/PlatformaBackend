using MediatR;
using Platforma.Application.Assignments;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Assignments
{
    public class AssignmentDetails
    {
        public class Query : IRequest<Result<Assignment>>
        {
            public required Guid AssignmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Assignment>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Assignment>> Handle(Query request, CancellationToken cancellationToken)
            {
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
                if (assignment == null) return Result<Assignment>.Failure("Assignment not found");

                return Result<Assignment>.Success(assignment);
            }
        }
    }
}
