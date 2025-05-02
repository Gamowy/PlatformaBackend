using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Users
{
    public class UserRegisterDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RepeatedPassword { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
