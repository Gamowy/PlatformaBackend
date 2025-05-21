using Microsoft.AspNetCore.Identity;
using Platforma.Domain;

namespace Platforma.Infrastructure
{
    public class DbSeed
    {
        public static async Task SeedData(DataContext dataContext)
        {
            await seedUsers(dataContext);
            await seedCourses(dataContext);
            await seedAssignments(dataContext);
            await seedAnwsers(dataContext);
        }

        private static async Task seedUsers(DataContext dataContext)
        {
            //Usunąć jeżeli wszyscy zrobią pull na branchu
            //if (dataContext.Users.Any())
            //{
            //    dataContext.Users.ExecuteDelete();
            //}

            //if (dataContext.Courses.Any())
            //{
            //    dataContext.Courses.ExecuteDelete();
            //}

            //if (dataContext.Assignments.Any())
            //{
            //    dataContext.Assignments.ExecuteDelete();
            //}

            if (dataContext.Users.Any()) return;

            var users = new List<User>
            {
                new User{ Username = "admin", Password = "admin", Name = "Pan admin", UserType = User.Roles.Administrator},
                new User{ Username = "nauczyciel", Password = "nauczyciel", Name = "Nauczyciel 1", UserType = User.Roles.Teacher},
                new User{ Username = "nauczyciel2", Password = "nauczyciel", Name = "Nauczyciel 2", UserType = User.Roles.Teacher},
                new User{ Username = "nauczyciel3", Password = "nauczyciel", Name = "Nauczyciel 3", UserType = User.Roles.Teacher},
                new User{ Username = "nauczyciel4", Password = "nauczyciel", Name = "Nauczyciel 4", UserType = User.Roles.Teacher},
                new User{ Username = "student", Password = "student", Name = "Student 1", UserType = User.Roles.Student, StudentIdNumber = "357891"},
                new User{ Username = "student2", Password = "student2", Name = "Student 2", UserType = User.Roles.Student, StudentIdNumber = "357892"},
                new User{ Username = "student3", Password = "student3", Name = "Student 3", UserType = User.Roles.Student, StudentIdNumber = "357893"},
                new User{ Username = "student4", Password = "student4", Name = "Student 4", UserType = User.Roles.Student, StudentIdNumber = "357894"},
                new User{ Username = "student5", Password = "student5", Name = "Student 5", UserType = User.Roles.Student, StudentIdNumber = "357895"},
                new User{ Username = "student6", Password = "student6", Name = "Student 6", UserType = User.Roles.Student, StudentIdNumber = "357896"},
                new User{ Username = "student7", Password = "student7", Name = "Student 7", UserType = User.Roles.Student, StudentIdNumber = "357897"},
                new User{ Username = "student8", Password = "student8", Name = "Student 8", UserType = User.Roles.Student, StudentIdNumber = "357898"},
                new User{ Username = "student9", Password = "student9", Name = "Student 9", UserType = User.Roles.Student, StudentIdNumber = "357899"}
            };

            foreach (var user in users)
            {
                user.Password = new PasswordHasher<User>().HashPassword(user, user.Password);
            }

            await dataContext.Users.AddRangeAsync(users);
            await dataContext.SaveChangesAsync();
        }

        private static async Task seedCourses(DataContext dataContext)
        {
            if (dataContext.Courses.Any()) return;
            if (!dataContext.Users.Any()) return;
            
            var teacher = dataContext.Users.Where(u => u.UserType == User.Roles.Teacher).First();
            var teacher4 = dataContext.Users.Where(u => u.Username == "nauczyciel4").First();
            var teacher2 = dataContext.Users.Where(u => u.Username == "nauczyciel2").First();


            var student = dataContext.Users.Where(u => u.UserType == User.Roles.Student).First();
            var student2 = dataContext.Users.Where(u => u.Username == "student2").First();
            var student3 = dataContext.Users.Where(u => u.Username == "student3").First();
            var student4 = dataContext.Users.Where(u => u.Username == "student4").First();


            if (teacher == null || student == null) return;

            var course = new Course { Name = "Test course 1", Description = "", Owner = teacher, AcademicYear = "24/25", Users = new List<User> { student } };
            var course2 = new Course { Name = "Test course 2", Description = "", Owner = teacher2, AcademicYear = "23/24", Users = new List<User> { student,student2,student3,student4 } };
            var course3 = new Course { Name = "Test course 3", Description = "", Owner = teacher4, AcademicYear = "22/23", Users = new List<User> { student3,teacher2 } };

            
            await dataContext.Courses.AddAsync(course);
            await dataContext.Courses.AddAsync(course2);
            await dataContext.Courses.AddAsync(course3);
            await dataContext.SaveChangesAsync();
        }

        private static async Task seedAssignments(DataContext dataContext)
        {
            //TODO: po dodaniu mechanizmów przesyłania plików wrócić i dodać właściwy FilePath
            if (dataContext.Assignments.Any()) return;
            if(!dataContext.Courses.Any()) return;

            var testCourse = dataContext.Courses.Where(c => c.Name == "Test course 2").First();

            if(testCourse == null) return;

            var assignment = new Assignment
            {
                Name = "TestLab",
                Content = "wykonaj polecenia zawarte w pliku Lab1.pdf i zamieść odpowiedź na patformie",
                Course = testCourse,
                OpenDate = DateTime.UtcNow.AddDays(-3),
                Deadline = DateTime.UtcNow.AddDays(4),
                FilePath = "Example/Assigments1/d020b3f1-c741-4d63-baff-c7f473433b26_Task.docx",
                AcceptedFileTypes = ".zip;.pdf;.docx.;.txt"
            };

            await dataContext.Assignments.AddAsync(assignment);
            await dataContext.SaveChangesAsync();
        }

        private static async Task seedAnwsers(DataContext dataContext)
        {
            //TODO: po dodaniu mechanizmów przesyłania plików wrócić i dodać właściwy FilePath
            if (dataContext.Answers.Any()) return;
            if (!dataContext.Assignments.Any()) return;

            var testAssignment = dataContext.Assignments.Where(a => a.Name == "TestLab").First();
            var testUser = dataContext.Users.Where(a => a.StudentIdNumber == "357891").First();

            if (testAssignment == null || testUser == null) return;

            var answer = new Answer
            {
                Assignment = testAssignment,
                User = testUser,
                FilePath = "Example/Assigments1/d020b3f1-c741-4d63-baff-c7f473433b26_Answer1.docx",
            };

            var testAssignment2 = dataContext.Assignments.Where(a => a.Name == "TestLab").First();
            var testUser2 = dataContext.Users.Where(a => a.StudentIdNumber == "357892").First();

            if (testAssignment2 == null || testUser2 == null) return;

            var answer2 = new Answer
            {
                Assignment = testAssignment2,
                User = testUser2,
                FilePath = "Example/Assigments1/d020b3f1-c741-4d63-baff-c7f473433b26_Answer2.pdf",
            };

            var testAssignment3 = dataContext.Assignments.Where(a => a.Name == "TestLab").First();
            var testUser3 = dataContext.Users.Where(a => a.StudentIdNumber == "357893").First();

            if (testAssignment3 == null || testUser3 == null) return;

            var answer3 = new Answer
            {
                Assignment = testAssignment3,
                User = testUser3,
                FilePath = "Example/Assigments1/d020b3f1-c741-4d63-baff-c7f473433b26_Answer3.txt",
            };


            await dataContext.AddAsync(answer);
            await dataContext.AddAsync(answer2);
            await dataContext.AddAsync(answer3);
            await dataContext.SaveChangesAsync();
        }
    }
}
