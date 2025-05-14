namespace Platforma.Application.Courses.DTOs
{
    public class CourseEditDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AcademicYear { get; set; }
        public Guid? OwnerId { get; set; }
    }
}
