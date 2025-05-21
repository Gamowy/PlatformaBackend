using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Platforma.Application.Accounts.DTOs;
using Platforma.Domain;
using Platforma.Infrastructure;

namespace Platforma.Application.Users
{
    public class Register
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required UserRegisterDTO UserRegisterDTO { get; set; }
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
                var found = _context.Users.Where(u => u.Username == request.UserRegisterDTO.UserName).FirstOrDefault();
                if (found != null)
                {
                    return Result<Unit?>.Failure("User with this username already exists");
                }

                var newUser = new User()
                {
                    Name = request.UserRegisterDTO.Name + " " + request.UserRegisterDTO.Surname,
                    Password = request.UserRegisterDTO.Password,
                    Username = request.UserRegisterDTO.UserName,
                    UserType = User.Roles.Student
                };
                newUser.Password = new PasswordHasher<User>().HashPassword(newUser, newUser.Password);
                _context.Users.Add(newUser);
                var reult = await _context.SaveChangesAsync() > 0;
                if (!reult) Result<Unit?>.Failure("Failed to add user");
                return Result<Unit?>.Success(Unit.Value);
            }

        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.UserRegisterDTO).SetValidator(new RegisterValidator());
            }
        }
    }
}
