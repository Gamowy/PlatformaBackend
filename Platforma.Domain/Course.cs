using System.Text.Json.Serialization;

namespace Platforma.Domain
{
    public class Course
    {
        public Guid Id { get; set; }
        public List<User>? Users { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
        [JsonIgnore]
        public User? Owner { get; set; }
        public string AcademicYear { get; set; }
        [JsonIgnore]
        public List<Assignment>? Assignments { get; set; }
    }
}
