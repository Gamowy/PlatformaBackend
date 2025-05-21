using FluentValidation;
using MediatR;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.CourseUsers
{
    public class AcceptUser
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
                CourseUser? courseUser = _context.CourseUsers.Where(cu => cu.UserID.Equals(request.UserId) && cu.CourseID.Equals(request.CourseId)).FirstOrDefault();
                if (courseUser == null) 
                    return Result<Unit?>.Failure("No request waiting");

                courseUser.Status = UserStatus.Accepted;

                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit?>.Failure("Failed to accept user.");
                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
