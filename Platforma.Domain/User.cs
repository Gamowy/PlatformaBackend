using System.Text.Json.Serialization;

namespace Platforma.Domain
{
    public enum UserType { Student, Teacher, Administrator }

    public class User 
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public List<Course> Courses { get; set; } 
        public List<Course> OwnedCourses { get; set; }
        public string? StudentIdNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public UserType UserType { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
