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
            public required string? SearchedPhrase { get; set; }
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
                var result = new List<Course>();
                if (request.SearchedPhrase != null && request.SearchedPhrase.Trim().Length > 0) 
                    result = await _context.Courses.Where(c => c.Name.Contains(request.SearchedPhrase)).Include(c => c.Owner).ToListAsync();
                else
                    result = await _context.Courses.Include(c => c.Owner).ToListAsync();

                return Result<List<Course>>.Success(result);
            }
        }
    }
}




