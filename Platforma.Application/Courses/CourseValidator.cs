using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Platforma.Application.Courses.DTOs;
using Platforma.Domain;

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
