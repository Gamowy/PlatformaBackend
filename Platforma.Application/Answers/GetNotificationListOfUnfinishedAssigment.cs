using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Domain;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Answers
{
    public class GetNotificationListOfUnfinishedAssigment
    {
        public class Query : IRequest<Result<List<NumberOfAnswersWithNameDTO>>>
        {
            public required Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<NumberOfAnswersWithNameDTO>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<NumberOfAnswersWithNameDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var courses = await _context.Courses
                    .Where(a => a.OwnerId == request.UserId)
                    .ToListAsync(cancellationToken);


                if (courses == null)
                {
                    return Result<List<NumberOfAnswersWithNameDTO>>.Failure("No course found");
                }

                var numberOfAnswersDtoList = new List<NumberOfAnswersWithNameDTO>();

                foreach (Course course in courses)
                {
                    var assignments = await _context.Assignments
                        .Where(a => a.CourseId == course.Id && a.AnswerRequired == true && a.Deadline < DateTime.Today)
                        .ToListAsync(cancellationToken);

                    var answers = await _context.Answers
                        .Where(a => a.Assignment.CourseId == course.Id)
                        .ToListAsync(cancellationToken);

                    var courseUser = await _context.CourseUsers
                        .Where(a => a.CourseID == course.Id)
                        .ToListAsync(cancellationToken);

                    if (assignments == null)
                    {
                        break;
                    }

                    foreach (var assignment in assignments)
                    {
                        var numberOfFinishedAnswers = answers.Count(a => a.AssignmentId == assignment.Id);
                        var numberOfUnfinishedAnswers = courseUser.Count() - numberOfFinishedAnswers;

                        numberOfAnswersDtoList.Add(new NumberOfAnswersWithNameDTO
                        {
                            AssignmentId = assignment.Id,
                            CourseId = course.Id,
                            FinishedAnswers = numberOfFinishedAnswers,
                            UnfinishedAnswers = numberOfUnfinishedAnswers,
                            CourseName = course.Name,
                            AssigmentName = assignment.Name,
                            Deadline = assignment.Deadline
                        });
                    }



                }
                return Result<List<NumberOfAnswersWithNameDTO>>.Success(numberOfAnswersDtoList);

            }
        }
        }
}
