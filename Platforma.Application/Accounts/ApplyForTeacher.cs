using MediatR;
using Microsoft.AspNetCore.Identity;
using Platforma.Application.Accounts.DTOs;
using Platforma.Domain;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Accounts
{
    public class ApplyForTeacher
    {
        public class Command : IRequest<Result<Unit>>
        {
            public required Guid UserId { get; set; }
            public required bool Withdraw { get; set; } = false;
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
                    if(!request.Withdraw && user.UserType != User.Roles.Student)
                        return Result<Unit>.Failure("You can't apply for teacher (you are teacher or you have already applied)");

                    if (request.Withdraw && user.UserType != User.Roles.TeacherApplicant)
                        return Result<Unit>.Failure("You didn't apply for teacher");

                    user.UserType = (request.Withdraw) ? User.Roles.Student : User.Roles.TeacherApplicant;

                    var result = await _context.SaveChangesAsync() > 0;
                    if (!result) return Result<Unit>.Failure("Failed to apply for teacher role.");

                    return Result<Unit>.Success(Unit.Value);
                }
                return Result<Unit>.Failure("no user");
            }
        }
    }
}
