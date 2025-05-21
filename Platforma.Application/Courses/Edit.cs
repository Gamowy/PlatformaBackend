using MediatR;
using Platforma.Application.Courses.DTOs;
using Platforma.Infrastructure;

namespace Platforma.Application.Courses
{
    public class Edit
    {

        public class Command : IRequest<Result<Unit?>>
        {
            public required Guid CourseId { get; set; }
            public required CourseEditDTO CourseDTO { get; set; }
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
                var course = await _context.Courses.FindAsync(request.CourseId);
                if (course == null) return Result<Unit?>.Failure("Course to update not found.");

                course.Name = request.CourseDTO.Name ?? course.Name;
                course.Description = request.CourseDTO.Description ?? course.Description;
                course.OwnerId = request.CourseDTO.OwnerId ?? course.OwnerId;
                course.AcademicYear= request.CourseDTO.AcademicYear ?? course.AcademicYear;

                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit?>.Failure("Failed to update course.");
                return Result<Unit?>.Success(Unit.Value);
            }
        }
    }
}
