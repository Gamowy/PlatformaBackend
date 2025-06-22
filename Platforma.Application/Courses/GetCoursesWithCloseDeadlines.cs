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
    public class GetCoursesWithCloseDeadlines
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
                // Get course IDs for the user
                var userCourseIds = await _context.CourseUsers
                    .Where(cu => cu.UserID == request.UserId && cu.Status == UserStatus.Accepted)
                    .Select(cu => cu.CourseID)
                    .ToListAsync(cancellationToken);
                if (userCourseIds.Count == 0)
                    return Result<List<Course>>.Failure("No courses found for the user.");

                var courses = await _context.Courses
                    .Where(c => userCourseIds.Contains(c.Id))
                    .ToListAsync(cancellationToken);
                if (courses.Count == 0)
                    return Result<List<Course>>.Failure("No courses found for the user.");

                // Get assignments with close deadlines
                var now = DateTime.UtcNow;
                var closeAssignments = await _context.Assignments
                    .Where(a => userCourseIds.Contains(a.CourseId) && a.Deadline > now && a.Deadline < now.AddDays(7))
                    .ToListAsync(cancellationToken);
                if (closeAssignments.Count == 0)
                    return Result<List<Course>>.Failure("No assignments with close deadlines found for the user's courses.");

                // Get answers for those assignments
                var closeAssignmentIds = closeAssignments.Select(a => a.Id).ToList();
                var answers = await _context.Answers
                    .Where(a => closeAssignmentIds.Contains(a.AssignmentId) && a.UserId == request.UserId)
                    .ToListAsync(cancellationToken);

                var result = courses
                    .Where(course => closeAssignments.Any(a => a.CourseId == course.Id && !answers.Any(ans => ans.AssignmentId == a.Id)))
                    .ToList();

                return Result<List<Course>>.Success(result);
            }
        }
    }
}
