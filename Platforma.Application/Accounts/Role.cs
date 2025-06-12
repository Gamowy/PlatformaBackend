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
    public class Role
    {
        public class Query : IRequest<Result<string>>
        {
            public required Guid UserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<string>>
        {
            private readonly DataContext _context;

            public Handler(DataContext dataContext)
            {
                _context = dataContext;
            }

            public async Task<Result<string>> Handle(Query request, CancellationToken cancellationToken)
            {
                string role = _context.Users.Where(u => u.Id == request.UserId).FirstOrDefault()?.UserType ?? "Student";


                return Result<string>.Success(role);
            }
        }
    }
}
