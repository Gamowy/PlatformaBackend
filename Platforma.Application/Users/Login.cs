using Azure.Core;
using MediatR;
using Platforma.Infrastructure;
using Platforma.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace Platforma.Application.Users
{
    public class Login
    {
        public class Query : IRequest<Result<String>>
        {
            public required UserLoginDTO UserLoginDTO {  get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<String>>
        {
            private readonly DataContext _context;
            private readonly IConfiguration _configuration;

            public Handler(DataContext dataContext, IConfiguration configuration)
            {
                _context = dataContext;
                _configuration = configuration;
            }

            public async Task<Result<String>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = _context.Users.Where(u => u.Username == request.UserLoginDTO.Username).FirstOrDefault();

                if (user != null)
                {
                    var hashed = new PasswordHasher<User>().HashPassword(user, user.Password);
                    if (new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.UserLoginDTO.Password) == PasswordVerificationResult.Success)
                        return Result<String>.Success(CreateToken(user));
                }
                     return Result<String>.Failure("wrong username or password");
            }

            private string CreateToken(User user)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
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
                
                return accessToken;
            }
        }
    }
}
