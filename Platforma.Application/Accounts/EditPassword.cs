using MediatR;
using Microsoft.AspNetCore.Identity;
using Platforma.Application.Accounts.DTOs;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Accounts
{
    public class EditPassword
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required PassChangeDTO PassResetDTO { get; set; }
            public required Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit?>>
        {
            private readonly DataContext _context;

            public Handler(DataContext dataContext)
            {
                _context = dataContext;
            }

            public async Task<Result<Unit?>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = _context.Users.Where(u => u.Id == request.UserId).FirstOrDefault();

                if (user != null)
                {
                    if (new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.PassResetDTO.OldPassword) != PasswordVerificationResult.Success)
                        return Result<Unit?>.Failure("Incorrect old password");

                    user.Password = new PasswordHasher<User>().HashPassword(user, request.PassResetDTO.NewPassword);

                    var result = await _context.SaveChangesAsync() > 0;
                    if (!result) return Result<Unit?>.Failure("Failed to change password.");

                    return Result<Unit?>.Success(Unit.Value);
                }
                return Result<Unit?>.Failure("no user");
            }
        }
    }
}
