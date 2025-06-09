using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Application.Courses.DTOs;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Courses
{
    public class UserList
    {
        public class Query : IRequest<Result<List<UserCourseDTO>>>
        {
            public required Guid CourseId { get; set; }
            public required Boolean onlyAccepted { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<UserCourseDTO>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<List<UserCourseDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses
                    .Include(c => c.Users)
                    .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

                if (course == null)
                {
                    return Result<List<UserCourseDTO>>.Failure("Course not found.");
                }
                List<UserCourseDTO> list;

                if (request.onlyAccepted)
                {
                    list = course.Users.Where(u =>(_context.CourseUsers.FirstOrDefault(cu => cu.CourseID == request.CourseId && cu.UserID == u.Id)?.Status ?? UserStatus.Awaiting) == UserStatus.Accepted)
                    .Select(u => new UserCourseDTO(u.Id, u.StudentIdNumber, u.Username, u.Name, u.UserType,
                        (_context.CourseUsers.Where(cu => cu.CourseID == request.CourseId && cu.UserID == u.Id).FirstOrDefault().Status ?? UserStatus.Awaiting)
                   )).ToList();
                }
                else
                {
                    list = course.Users.Select(u => new UserCourseDTO(u.Id, u.StudentIdNumber, u.Username, u.Name, u.UserType,
                        (_context.CourseUsers.Where(cu => cu.CourseID == request.CourseId && cu.UserID == u.Id).FirstOrDefault().Status ?? UserStatus.Awaiting)
                        )).ToList();
                }

                


                return Result<List<UserCourseDTO>>.Success(list);
            }
        }
    }
}
