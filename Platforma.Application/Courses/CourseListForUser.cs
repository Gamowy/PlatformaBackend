using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Domain;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Courses
{
    public class CourseListForUser
    {
        public class Query : IRequest<Result<List<Course>>>
        {
            public required Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Course>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<List<Course>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userCourses = await _context.CourseUsers.Where(uc => uc.UserID.Equals(request.UserId) && uc.Status == UserStatus.Accepted).ToListAsync(cancellationToken);
                if (userCourses == null)
                    return Result<List<Course>>.Failure("Can't get user courses");

                var courseIds = userCourses.Select(uc => uc.CourseID).ToList();

                var result = await _context.Courses.Where(c => courseIds.Contains(c.Id) || c.OwnerId.Equals(request.UserId)).Include(c => c.Owner).ToListAsync(cancellationToken);

                return Result<List<Course>>.Success(result);
            }
        }
    }
}
