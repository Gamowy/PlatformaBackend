using FluentValidation;
using MediatR;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.CourseUsers
{
    public class AssignRequest
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid CourseId { get; set; }
            public required Guid UserId { get; set; }
            public UserStatus status { get; set; } = UserStatus.Awaiting;
        }


        public class Handler : IRequestHandler<Command, Result<Unit?>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }


            public async Task<Result<Unit?>> Handle(Command request, CancellationToken cancellationToken)
            {
                CourseUser? exists = _context.CourseUsers
                    .Where(cu => cu.UserID.Equals(request.UserId) && cu.CourseID.Equals(request.CourseId)).FirstOrDefault();

                if (exists != null) return Result<Unit?>.Failure("user already assigned to course or request incorrect");

                Course? course = _context.Courses.Where(c => c.Id.Equals(request.CourseId)).FirstOrDefault();
                if(course != null && course.OwnerId.Equals(request.UserId))
                    return Result<Unit?>.Failure("Owner can't be course participant");

                CourseUser assignation = new CourseUser
                {
                    CourseID = request.CourseId,
                    UserID = request.UserId,
                    Status = request.status
                };

                _context.CourseUsers.Add(assignation);
                var reult = await _context.SaveChangesAsync() > 0;
                return Result<Unit?>.Success(Unit.Value);
            }

        }
    }
}
