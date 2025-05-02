using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Platforma.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //if (dataContext.Users.Any() && dataContext.Users.Where(u => u.Password == "student").First() != null) {
            //    dataContext.Users.ExecuteDelete();
            //}

            if (dataContext.Users.Any()) return;

            var users = new List<User>
            {
                new User{ Username = "admin", Password = "admin", Name = "Pan admin", UserType = User.Roles.Administrator},
                new User{ Username = "nauczyciel", Password = "nauczyciel", Name = "Nauczyciel 1", UserType = User.Roles.Teacher},
                new User{ Username = "student", Password = "student", Name = "Student 1", UserType = User.Roles.Student, StudentIdNumber = "357895"}
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
            var student = dataContext.Users.Where(u => u.UserType == User.Roles.Student).First();

            if(teacher == null || student == null) return;

            var course = new Course { Name = "Test course", Description = "", Owner = teacher, AcademicYear = "24/25", Users = new List<User> { student } };
            await dataContext.Courses.AddAsync(course);
            await dataContext.SaveChangesAsync();
        }

        private static async Task seedAssignments(DataContext dataContext)
        {
            //TODO: po dodaniu mechanizmów przesyłania plików wrócić i dodać właściwy FilePath
            if (dataContext.Assignments.Any()) return;
            if(!dataContext.Courses.Any()) return;

            var testCourse = dataContext.Courses.Where(c => c.Name == "Test course").First();

            if(testCourse == null) return;

            var assignment = new Assignment
            {
                Name = "TestLab",
                Content = "wykonaj polecenia zawarte w pliku Lab1.pdf i zamieść odpowiedź na patformie",
                Course = testCourse,
                OpenDate = DateTime.Now.AddDays(-3),
                Deadline = DateTime.Now.AddDays(4),
                FilePath = "???",
                AcceptedFileTypes = ".zip;.pdf;.docx"
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
            var testUser = dataContext.Users.Where(a => a.StudentIdNumber == "357895").First();

            if (testAssignment == null || testUser == null) return;

            var answer = new Answer { 
                Assignment = testAssignment,
                User = testUser,
                SubmittedDate = DateTime.Now,
                FilePath = "???"
            };

            await dataContext.AddAsync(answer);
            await dataContext.SaveChangesAsync();
        }
    }
}
