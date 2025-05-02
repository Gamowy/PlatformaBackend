using MediatR;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Platforma.Domain;

namespace Platforma.Application.Courses
{
    public class Details
    {
        public class Query : IRequest<Result<Course>>
        {
            public Guid id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Course>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Course>> Handle(Query request, CancellationToken cancelllationToken)
            {
                var course = await _context.Courses.FindAsync(request.id);

                return Result<Course>.Success(course);
            }
        }
    }
}
