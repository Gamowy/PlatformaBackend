using FluentValidation;
using MediatR;
using Platforma.Application.Courses.DTOs;
using Platforma.Domain;
using Platforma.Infrastructure;


namespace Platforma.Application.Courses
{
    public class Create
    {
        public class Command : IRequest<Result<Unit?>>
        {
            public required CourseCreateDTO CourseDTO { get; set; }
            public required Guid UserId { get; set; }
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
                User? owner = _context.Users.Where(u => u.Id.Equals(request.UserId)).First();

                if (owner == null)
                    return Result<Unit?>.Failure("Invalid Owner Id");

                Course newCourse = new Course
                {
                    AcademicYear = request.CourseDTO.AcademicYear,
                    Description = request.CourseDTO.Description,
                    Name = request.CourseDTO.Name,
                    Owner = owner
                };

                _context.Courses.Add(newCourse);
                var result = await _context.SaveChangesAsync() > 0;
                return Result<Unit?>.Success(Unit.Value);
            }

        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.CourseDTO).SetValidator(new CourseValidator());
            }
        }
    }
}
