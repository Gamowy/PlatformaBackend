using MediatR;
using Platforma.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Courses
{
    //TODO: dorobić usuwanie przypisań użytkowników i zadań w momencie usuwania kursu 
    public class Delete
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit?>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit?>> Handle(Command request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.Id);

                _context.Remove(course);
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit?>.Failure("Failed to delete the course");

                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
