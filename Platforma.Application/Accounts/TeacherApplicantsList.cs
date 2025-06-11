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
    public class TeacherApplicantsList
    {
        public class Query : IRequest<Result<List<User>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<User>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext dataContext)
            {
                _context = dataContext;
            }

            public async Task<Result<List<User>>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<User> users = _context.Users.Where(u => u.UserType == User.Roles.TeacherApplicant).ToList();


                return Result<List<User>>.Success(users);
            }
        }
    }
}
