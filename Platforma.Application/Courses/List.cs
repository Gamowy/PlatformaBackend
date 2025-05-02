using MediatR;
using Platforma.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Platforma.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Courses
{
    public class CourseList
    {
        public class Query : IRequest<Result<List<Course>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<Course>>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<List<Course>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var result = await _context.Courses.ToListAsync();
                return Result<List<Course>>.Success(result);
            }
        }
    }
}




