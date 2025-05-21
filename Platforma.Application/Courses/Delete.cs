using MediatR;
using Platforma.Domain;
using Platforma.Infrastructure;

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
                if (course == null) return Result<Unit?>.Failure("Course to delete not found");

                _context.Remove(course);
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit?>.Failure("Failed to delete the course");

                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
