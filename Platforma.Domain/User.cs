using System.Text.Json.Serialization;

namespace Platforma.Domain
{

    public class User 
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public List<Course> Courses { get; set; }
        [JsonIgnore]
        public List<Course> OwnedCourses { get; set; }
        public string? StudentIdNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string UserType { get; set; }
        [JsonIgnore]
        public List<Answer> Answers { get; set; }

        public struct Roles
        {
            public const string Administrator = "Administrator";
            public const string Teacher = "Teacher";
            public const string TeacherApplicant = "TeacherApplicant";
            public const string Student = "Student";
        }
    }
}
