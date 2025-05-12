using MediatR;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Answers
{
    public class GetAnswerDetails
    {
        public class Query : IRequest<Result<Answer>>
        {
            public required Guid AnswerId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Answer>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Answer>> Handle(Query request, CancellationToken cancellationToken)
            {
                var answer = await _context.Answers.FindAsync(request.AnswerId);
                if (answer == null) return Result<Answer>.Failure("Answer not found");

                return Result<Answer>.Success(answer);
            }
        }
    }
}
