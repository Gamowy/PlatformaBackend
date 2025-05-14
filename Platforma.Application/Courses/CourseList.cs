using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Domain;
using Platforma.Infrastructure;

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




