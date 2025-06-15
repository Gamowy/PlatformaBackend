using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Answers
{
    public class GetNumberOfAnswers
    {
        public class Query : IRequest<Result<List<NumberOfAnswersDTO>>>
        {
            public required Guid CourseId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<NumberOfAnswersDTO>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<NumberOfAnswersDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var answers = await _context.Answers
                    .Where(a => a.Assignment.CourseId == request.CourseId)
                    .ToListAsync(cancellationToken);
                if (answers == null)
                {
                    return Result<List<NumberOfAnswersDTO>>.Failure("No answers found for the specified course.");
                }

                var assignments = await _context.Assignments
                    .Where(a => a.CourseId == request.CourseId && a.AnswerRequired == true)
                    .ToListAsync(cancellationToken);
                if (assignments == null)
                {
                    return Result<List<NumberOfAnswersDTO>>.Failure("No assignments found for the specified course.");
                }

                var numberOfAnswersDtoList = new List<NumberOfAnswersDTO>();

                foreach(var assignment in assignments)
                {
                    var numberOfUnmarkedAnswers = answers.Count(a => a.AssignmentId == assignment.Id && a.Mark == null);
                    var numberOfMarkedAnswers = answers.Count(a => a.AssignmentId == assignment.Id && a.Mark != null);
                    numberOfAnswersDtoList.Add(new NumberOfAnswersDTO
                    {
                        AssignmentId = assignment.Id,
                        UnmarkedAnswers = numberOfUnmarkedAnswers,
                        MarkedAnswers = numberOfMarkedAnswers
                    });
                }
                return Result<List<NumberOfAnswersDTO>>.Success(numberOfAnswersDtoList);
            }
        }
    }
}
