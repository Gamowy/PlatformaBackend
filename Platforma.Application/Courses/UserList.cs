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
    public class UserList
    {
        public class Query : IRequest<Result<List<User>>>
        {
            public required Guid CourseId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<User>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<List<User>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses
                    .Include(c => c.Users)
                    .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

                if (course == null)
                {
                    return Result<List<User>>.Failure("Course not found.");
                }
                if (course.Users == null)
                {
                    return Result<List<User>>.Failure("No users found for this course.");
                }

                return Result<List<User>>.Success(course.Users);
            }
        }
    }
}
