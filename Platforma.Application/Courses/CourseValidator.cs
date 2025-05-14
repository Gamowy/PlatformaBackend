using FluentValidation;
using Platforma.Application.Courses.DTOs;

namespace Platforma.Application.Courses
{
    public class CourseValidator : AbstractValidator<CourseCreateDTO>
    {
        public CourseValidator() {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name of course is required");
            //RuleFor(x => x.Description).NotEmpty().WithMessage("Description of course is required");
            RuleFor(x => x.AcademicYear).NotEmpty().WithMessage("Accademic year of is required");
        }
    }
}
