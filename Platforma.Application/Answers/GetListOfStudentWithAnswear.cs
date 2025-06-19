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
    public class GetListOfStudentWithAnswear
    {
        public class Query : IRequest<Result<List<ListOfStudentWithAnswearDTO>>>
        {
            public required Guid CourseId { get; set; }
            public required Guid AssigmentId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<ListOfStudentWithAnswearDTO>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<ListOfStudentWithAnswearDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var courseUser = await _context.CourseUsers
                          .Where(a => a.CourseID == request.CourseId)
                          .ToListAsync(cancellationToken);


                if (courseUser == null)
                {
                    return Result<List<ListOfStudentWithAnswearDTO>>.Failure("No user found");
                }

                var numberOfAnswersDtoList = new List<ListOfStudentWithAnswearDTO>();

                foreach (CourseUser user in courseUser)
                {
                    var answer = await _context.Answers
                        .Where(a => a.Assignment.Id == request.AssigmentId && a.UserId == user.UserID)
                        .FirstOrDefaultAsync();

                    if (answer != null)
                    {
                        numberOfAnswersDtoList.Add(new ListOfStudentWithAnswearDTO
                        {
                            UserId = user.UserID,
                            AnswerId = answer.Id,
                            IsSubmitted = true,
                            UserName = answer.User.Username,
                            SubmittedDate = answer.SubmittedDate,
                            Mark = answer.Mark,
                            Comment = answer.Comment
                        });
                    }
                    else 
                    {
                        var userWithUserName = await _context.Users
                         .Where(a => a.Id == user.UserID)
                         .FirstOrDefaultAsync();

                        numberOfAnswersDtoList.Add(new ListOfStudentWithAnswearDTO
                        {
                            UserId = user.UserID,
                            AnswerId = null,
                            IsSubmitted = false,
                            UserName = userWithUserName.Username,
                            SubmittedDate = null,
                            Mark = null,
                            Comment = null
                        });
                    }

        
                



                }
                return Result<List<ListOfStudentWithAnswearDTO>>.Success(numberOfAnswersDtoList);

            }
        }
    }
}
