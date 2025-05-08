using MediatR;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Answers
{
    public class AnswerAssignmentId
    {
        public class Query : IRequest<Result<Guid>>
        {
            public required Guid AnswerId { get; set; }
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
                var answer = await _context.Answers.FindAsync(request.AnswerId);
                if (answer == null) return Result<Guid>.Failure("Answer not found");
                return Result<Guid>.Success(answer.AssignmentId);
            }
        }
    }
}
