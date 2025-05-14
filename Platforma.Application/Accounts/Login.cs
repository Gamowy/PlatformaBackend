using MediatR;
using Platforma.Infrastructure;
using Platforma.Domain;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace Platforma.Application.Users
{
    public class Login
    {
        public class Query : IRequest<Result<TokenDTO>>
        {
            public required UserLoginDTO UserLoginDTO {  get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<TokenDTO>>
        {
            private readonly DataContext _context;
            private readonly IConfiguration _configuration;

            public Handler(DataContext dataContext, IConfiguration configuration)
            {
                _context = dataContext;
                _configuration = configuration;
            }

            public async Task<Result<TokenDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = _context.Users.Where(u => u.Username == request.UserLoginDTO.Username).FirstOrDefault();

                if (user != null)
                {
                    if (new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.UserLoginDTO.Password) == PasswordVerificationResult.Success)
                        return Result<TokenDTO>.Success(await CreateToken(user));
                }
                    return Result<TokenDTO>.Failure("wrong username or password");
            }

            private Task<TokenDTO> CreateToken(User user)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.UserType)
                };

                var issuer = _configuration["JwtConfig:Issuer"];
                var audience = _configuration["JwtConfig:Audience"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]!));
                var expiryTimeStamp = DateTime.UtcNow.AddMinutes(30);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expiryTimeStamp,
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(securityToken);

                var tokenJson = new TokenDTO(accessToken);

                return Task.FromResult(tokenJson);
            }
        }
    }
}
