using MediatR;
using Platforma.Infrastructure;

namespace Platforma.Application.Answers
{
    public class MarkAnswer
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid AnswerId { get; set; }
            public required MarkAnswerDTO MarkAnswerDTO { get; set; }
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
                var answer = await _context.Answers.FindAsync(request.AnswerId);
                if (answer == null) return Result<Unit?>.Failure("Answer not found");
                answer.Mark = request.MarkAnswerDTO.Mark;
                answer.Comment = request.MarkAnswerDTO.Comment;
                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit?>.Failure("Failed to mark answer");
                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
