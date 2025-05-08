using Platforma.Domain;

namespace Platforma.Application.Courses
{
    public class UserCourseDTO
    {
        public Guid Id { get; }
        public string? StudentIdNumber { get; }
        public string Username { get; }
        public string Name { get; }
        public string UserType { get; }
        public UserStatus status { get; set; }

        public UserCourseDTO(Guid id, string? studentIdNumber, string username, string name, string userType, UserStatus status)
        {
            Id = id;
            StudentIdNumber = studentIdNumber;
            Username = username;
            Name = name;
            UserType = userType;
            this.status = status;
        }
    }
}
