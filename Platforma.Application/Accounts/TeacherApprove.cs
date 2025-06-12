using MediatR;
using Platforma.Domain;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Accounts
{
    public class TeacherApprove
    {
        public class Command : IRequest<Result<Unit>>
        {
            public required Guid UserId { get; set; }
            public required bool Approve { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext dataContext)
            {
                _context = dataContext;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = _context.Users.Where(u => u.Id == request.UserId).FirstOrDefault();

                if (user != null)
                {
                    if (user.UserType != User.Roles.TeacherApplicant)
                        return Result<Unit>.Failure("You can't approve user for teacher");

                    user.UserType = request.Approve ? User.Roles.Teacher : User.Roles.Student;

                    var result = await _context.SaveChangesAsync() > 0;
                    if (!result) return Result<Unit>.Failure("Failed to apply for teacher role.");

                    return Result<Unit>.Success(Unit.Value);
                }
                return Result<Unit>.Failure("no user");
            }
        }
    }
}
