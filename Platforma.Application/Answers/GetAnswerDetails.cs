using MediatR;
using Platforma.Infrastructure;

namespace Platforma.Application.Answers
{
    public class GetAnswerDetails
    {
        public class Query : IRequest<Result<AnswerDTOResponse>>
        {
            public required Guid AnswerId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<AnswerDTOResponse>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<AnswerDTOResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var answer = await _context.Answers.FindAsync(request.AnswerId);
                if (answer == null) return Result<AnswerDTOResponse>.Failure("Answer not found");

                var answerDto = new AnswerDTOResponse
                {
                    Id = answer.Id,
                    AssignmentId = answer.AssignmentId,
                    UserId = answer.UserId,
                    SubmittedDate = answer.SubmittedDate,
                    Comment = answer.Comment,
                    Mark = answer.Mark,
                    FilePath = answer.FilePath,
                };
                return Result<AnswerDTOResponse>.Success(answerDto);
            }
        }
    }
}
