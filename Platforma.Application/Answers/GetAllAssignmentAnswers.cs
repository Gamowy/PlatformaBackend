using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Application.Assignments;
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
        public class Query : IRequest<Result<List<AnswerDTOResponse>>>
        {
            public required Guid AssignmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<AnswerDTOResponse>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<AnswerDTOResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var assignment = await _context.Assignments.FindAsync(request.AssignmentId);
                if (assignment == null) return Result<List<AnswerDTOResponse>>.Failure("Course not found");
                
                var answers = await _context.Answers
                    .Where(x => x.AssignmentId == request.AssignmentId)
                    .ToListAsync(cancellationToken);
                if (answers == null || answers.Count == 0) return Result<List<AnswerDTOResponse>>.Failure("No answers found for this assignment");

                var answersDTOs = answers.Select(answers => new AnswerDTOResponse
                {
                    Id = answers.Id,
                    AssignmentId = answers.AssignmentId,
                    UserId = answers.UserId,
                    SubmittedDate = answers.SubmittedDate,
                    Comment = answers.Comment,
                    Mark = answers.Mark,
                    FilePath = answers.FilePath,
                }).ToList();
                return Result<List<AnswerDTOResponse>>.Success(answersDTOs);
            }
        }
    }
}
