using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Application.Courses.DTOs;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Answers
{
    public class GetAllAnswerForUserInCourse
    {
        public class Query : IRequest<Result<List<Answer>>>
        {
            public required Guid CourseId { get; set; }
            public required Guid UserId { get; set; }
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
                var course = await _context.Courses.FindAsync(request.CourseId);
                if (course == null) return Result<List<Answer>>.Failure("Course not found");


                var answers = await _context.Answers
                    .Where(a => _context.Assignments
                        .Where(x => x.CourseId == request.CourseId)
                        .Select(x => x.Id)
                        .Contains(a.AssignmentId)
                        && a.UserId == request.UserId)
                    .ToListAsync(cancellationToken);

                if (answers == null || answers.Count == 0) return Result<List<Answer>>.Failure("No answers found for this user in this course");

                return Result<List<Answer>>.Success(answers);

            }
        }
    }
}
