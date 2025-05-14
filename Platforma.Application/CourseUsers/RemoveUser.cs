using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Infrastructure;

namespace Platforma.Application.CourseUsers
{
    public class RemoveUser
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid CourseId { get; set; }
            public required Guid UserId { get; set; }
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
                var courseUser = await _context.CourseUsers.Where(cu => cu.CourseID.Equals(request.CourseId) && cu.UserID.Equals(request.UserId)).FirstOrDefaultAsync();

                if (courseUser == null) return Result<Unit?>.Failure("User not found");

                _context.Remove(courseUser);
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit?>.Failure("Failed to remove user from course");

                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
