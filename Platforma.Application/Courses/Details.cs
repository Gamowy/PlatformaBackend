using MediatR;
using Microsoft.EntityFrameworkCore;
using Platforma.Domain;
using Platforma.Infrastructure;

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
                var course = await _context.Courses.Include(c=>c.Users).Where(c => c.Id.Equals(request.id)).FirstOrDefaultAsync();

                if (course == null) return Result<Course>.Failure("Course not found");

                return Result<Course>.Success(course);
            }
        }
    }
}
