using FluentValidation;
using Platforma.Application.Accounts.DTOs;

namespace Platforma.Application.Users
{
    public class RegisterValidator : AbstractValidator<UserRegisterDTO>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Your name is required");
            RuleFor(x => x.Surname).NotEmpty().WithMessage("Your surname is required");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required").Length(3, 20).WithMessage("Username must be between 3 and 20 characters");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.RepeatedPassword).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.RepeatedPassword).Equal(x => x.Password).WithMessage("Passwords don't match");
        }
    }
}
