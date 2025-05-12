using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Application.Assignments;
using Platforma.Domain;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Answers
{
    public class GetAllAssignmentAnswers
    {
        public class Query : IRequest<Result<List<Answer>>>
        {
            public required Guid AssignmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Answer>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<Answer>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
                if (assignment == null) return Result<List<Answer>>.Failure("Course not found");
                
                var answers = await _context.Answers
                    .Where(x => x.AssignmentId == request.AssignmentId)
                    .ToListAsync(cancellationToken);
                if (answers == null || answers.Count == 0) return Result<List<Answer>>.Failure("No answers found for this assignment");

                return Result<List<Answer>>.Success(answers);
            }
        }
    }
}
