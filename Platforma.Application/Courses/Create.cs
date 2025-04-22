using MediatR;
using Platforma.Infrastructure;
using Platforma.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;


namespace Platforma.Application.Courses
{
    public class Create
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
                _context.Courses.Add(request.Course);
                var reult = await _context.SaveChangesAsync() > 0;
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
