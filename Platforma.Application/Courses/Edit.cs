using FluentValidation;
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
    public class Edit
    {

        public class Command : IRequest<Result<Unit>>
        {
            public required Course Course { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var course = await _context.Courses.FindAsync(request.Course.Id);
                if (course == null) return null;

                course.Name = request.Course.Name ?? course.Name;
                course.Description = request.Course.Description ?? course.Description;
                course.OwnerId = request.Course.OwnerId;
                course.AcademicYear= request.Course.AcademicYear ?? course.AcademicYear;

                var result = await _context.SaveChangesAsync() > 0;
                if (!result) return Result<Unit>.Failure("Failed to update course.");
                return Result<Unit>.Success(Unit.Value);
            }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Course).SetValidator(new CourseValidator());
            }
        }
    }
}
